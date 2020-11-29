using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace MinecraftClient.ChatBots
{
    /// <summary>
    /// ChatBot for storing and delivering Mails
    /// </summary>
    public class Mailer : ChatBot
    {
        /// <summary>
        /// Holds the list of ignored players
        /// </summary>
        private class IgnoreList : HashSet<string>
        {
            /// <summary>
            /// Read ignore list from file
            /// </summary>
            /// <param name="filePath">Path to the ignore list</param>
            /// <returns>Ignore list</returns>
            public static IgnoreList FromFile(string filePath)
            {
                IgnoreList ignoreList = new IgnoreList();
                foreach (string line in FileMonitor.ReadAllLinesWithRetries(filePath))
                {
                    if (!line.StartsWith("#"))
                    {
                        string entry = line.ToLower();
                        if (!ignoreList.Contains(entry))
                            ignoreList.Add(entry);
                    }
                }
                return ignoreList;
            }

            /// <summary>
            /// Save ignore list to file
            /// </summary>
            /// <param name="filePath">Path to destination file</param>
            public void SaveToFile(string filePath)
            {
                List<string> lines = new List<string>();
                lines.Add("#被忽略的玩家:");
                foreach (string player in this)
                    lines.Add(player);
                FileMonitor.WriteAllLinesWithRetries(filePath, lines);
            }
        }

        /// <summary>
        /// Holds the Mail database: a collection of Mails sent from a player to another player
        /// </summary>
        private class MailDatabase : List<Mail>
        {
            /// <summary>
            /// Read mail database from file
            /// </summary>
            /// <param name="filePath">Path to the database</param>
            /// <returns>Mail database</returns>
            public static MailDatabase FromFile(string filePath)
            {
                MailDatabase database = new MailDatabase();
                Dictionary<string, Dictionary<string, string>> iniFileDict = INIFile.ParseFile(FileMonitor.ReadAllLinesWithRetries(filePath));
                foreach (KeyValuePair<string, Dictionary<string, string>> iniSection in iniFileDict)
                {
                    //iniSection.Key is "mailXX" but we don't need it here
                    string sender = iniSection.Value["sender"];
                    string recipient = iniSection.Value["recipient"];
                    string content = iniSection.Value["content"];
                    DateTime timestamp = DateTime.Parse(iniSection.Value["timestamp"]);
                    bool anonymous = INIFile.Str2Bool(iniSection.Value["anonymous"]);
                    database.Add(new Mail(sender, recipient, content, anonymous, timestamp));
                }
                return database;
            }

            /// <summary>
            /// Save mail database to file
            /// </summary>
            /// <param name="filePath">Path to destination file</param>
            public void SaveToFile(string filePath)
            {
                Dictionary<string, Dictionary<string, string>> iniFileDict = new Dictionary<string, Dictionary<string, string>>();
                int mailCount = 0;
                foreach (Mail mail in this)
                {
                    mailCount++;
                    Dictionary<string, string> iniSection = new Dictionary<string, string>();
                    iniSection["sender"] = mail.Sender;
                    iniSection["recipient"] = mail.Recipient;
                    iniSection["content"] = mail.Content;
                    iniSection["timestamp"] = mail.DateSent.ToString();
                    iniSection["anonymous"] = mail.Anonymous.ToString();
                    iniFileDict["mail" + mailCount] = iniSection;
                }
                FileMonitor.WriteAllLinesWithRetries(filePath, INIFile.Generate(iniFileDict, "邮箱数据库"));
            }
        }

        /// <summary>
        /// Represents a Mail sent from a player to another player
        /// </summary>
        private class Mail
        {
            private string sender;
            private string senderLower;
            private string recipient;
            private string recipientLower;
            private string message;
            private DateTime datesent;
            private bool delivered;
            private bool anonymous;

            public Mail(string sender, string recipient, string message, bool anonymous, DateTime datesent)
            {
                this.sender = sender;
                this.senderLower = sender.ToLower();
                this.recipient = recipient;
                this.recipientLower = recipient.ToLower();
                this.message = message;
                this.datesent = datesent;
                this.delivered = false;
                this.anonymous = anonymous;
            }

            public string Sender { get { return sender; } }
            public string SenderLowercase { get { return senderLower; } }
            public string Recipient { get { return recipient; } }
            public string RecipientLowercase { get { return recipientLower; } }
            public string Content { get { return message; } }
            public DateTime DateSent { get { return datesent; } }
            public bool Delivered { get { return delivered; } }
            public bool Anonymous { get { return anonymous; } }
            public void setDelivered() { delivered = true; }

            public override string ToString()
            {
                return String.Format("{0} {1} {2} {3}", Sender, Recipient, Content, DateSent);
            }
        }

        // Internal variables
        private int maxMessageLength = 0;
        private DateTime nextMailSend = DateTime.Now;
        private MailDatabase mailDatabase = new MailDatabase();
        private IgnoreList ignoreList = new IgnoreList();
        private FileMonitor mailDbFileMonitor;
        private FileMonitor ignoreListFileMonitor;
        private object readWriteLock = new object();

        /// <summary>
        /// Initialization of the Mailer bot
        /// </summary>
        public override void Initialize()
        {
            LogDebugToConsole("邮箱机器人设置:");
            LogDebugToConsole(" - 数据库文件: " + Settings.Mailer_DatabaseFile);
            LogDebugToConsole(" - 忽略列表: " + Settings.Mailer_IgnoreListFile);
            LogDebugToConsole(" - 公开互动者: " + Settings.Mailer_PublicInteractions);
            LogDebugToConsole(" - 每个玩家最多发送的邮件: " + Settings.Mailer_MaxMailsPerPlayer);
            LogDebugToConsole(" - 最大数据库大小: " + Settings.Mailer_MaxDatabaseSize);
            LogDebugToConsole(" - 邮件保留时间: " + Settings.Mailer_MailRetentionDays + " 天");

            if (Settings.Mailer_MaxDatabaseSize <= 0)
            {
                LogToConsole("无法启用邮件功能: 邮件数据库最大大小必须大于0!");
                UnloadBot();
                return;
            }

            if (Settings.Mailer_MaxMailsPerPlayer <= 0)
            {
                LogToConsole("无法启用邮件功能: 每个玩家最大发送的邮件数量必须大于0!");
                UnloadBot();
                return;
            }

            if (Settings.Mailer_MailRetentionDays <= 0)
            {
                LogToConsole("无法启用邮件功能: 邮件保存时间必须大于0.");
                UnloadBot();
                return;
            }

            if (!File.Exists(Settings.Mailer_DatabaseFile))
            {
                LogToConsole("创建新的数据库文件: " + Path.GetFullPath(Settings.Mailer_DatabaseFile));
                new MailDatabase().SaveToFile(Settings.Mailer_DatabaseFile);
            }

            if (!File.Exists(Settings.Mailer_IgnoreListFile))
            {
                LogToConsole("创建新的忽略列表文件: " + Path.GetFullPath(Settings.Mailer_IgnoreListFile));
                new IgnoreList().SaveToFile(Settings.Mailer_IgnoreListFile);
            }

            lock (readWriteLock)
            {
                LogDebugToConsole("正在加载数据库文件: " + Path.GetFullPath(Settings.Mailer_DatabaseFile));
                mailDatabase = MailDatabase.FromFile(Settings.Mailer_DatabaseFile);

                LogDebugToConsole("正在加载忽略列表文件: " + Path.GetFullPath(Settings.Mailer_IgnoreListFile));
                ignoreList = IgnoreList.FromFile(Settings.Mailer_IgnoreListFile);
            }

            //Initialize file monitors. In case the bot needs to unload for some reason in the future, do not forget to .Dispose() them
            mailDbFileMonitor = new FileMonitor(Path.GetDirectoryName(Settings.Mailer_DatabaseFile), Path.GetFileName(Settings.Mailer_DatabaseFile), FileMonitorCallback);
            ignoreListFileMonitor = new FileMonitor(Path.GetDirectoryName(Settings.Mailer_IgnoreListFile), Path.GetFileName(Settings.Mailer_IgnoreListFile), FileMonitorCallback);

            RegisterChatBotCommand("mailer", "子命令: getmails, addignored, getignored, removeignored", ProcessInternalCommand);
        }

        /// <summary>
        /// Standard settings for the bot.
        /// </summary>
        public override void AfterGameJoined()
        {
            maxMessageLength = GetMaxChatMessageLength()
                - 44 // Deduct length of "/ 16CharPlayerName 16CharPlayerName mailed: "
                - Settings.PrivateMsgsCmdName.Length; // Deduct length of "tell" command
        }

        /// <summary>
        /// Process chat messages from the server
        /// </summary>
        public override void GetText(string text)
        {
            string message = "";
            string username = "";
            text = GetVerbatim(text);

            if (IsPrivateMessage(text, ref message, ref username) || (Settings.Mailer_PublicInteractions && IsChatMessage(text, ref message, ref username)))
            {
                string usernameLower = username.ToLower();
                if (!ignoreList.Contains(usernameLower))
                {
                    string command = message.Split(' ')[0].ToLower();
                    switch (command)
                    {
                        case "mail":
                        case "tellonym":
                            if (usernameLower != GetUsername().ToLower()
                                && mailDatabase.Count < Settings.Mailer_MaxDatabaseSize
                                && mailDatabase.Where(mail => mail.SenderLowercase == usernameLower).Count() < Settings.Mailer_MaxMailsPerPlayer)
                            {
                                Queue<string> args = new Queue<string>(Command.getArgs(message));
                                if (args.Count >= 2)
                                {
                                    bool anonymous = (command == "tellonym");
                                    string recipient = args.Dequeue();
                                    message = string.Join(" ", args);

                                    if (IsValidName(recipient))
                                    {
                                        if (message.Length <= maxMessageLength)
                                        {
                                            Mail mail = new Mail(username, recipient, message, anonymous, DateTime.Now);
                                            LogToConsole("Saving message: " + mail.ToString());
                                            lock (readWriteLock)
                                            {
                                                mailDatabase.Add(mail);
                                                mailDatabase.SaveToFile(Settings.Mailer_DatabaseFile);
                                            }
                                            SendPrivateMessage(username, "信息已保存!");
                                        }
                                        else SendPrivateMessage(username, "你的信息长度不能大于 " + maxMessageLength + " 字节.");
                                    }
                                    else SendPrivateMessage(username, "'" + recipient + "' 不是一个有效的玩家名.");
                                }
                                else SendPrivateMessage(username, "使用方法: " + command + " <玩家名> <信息>");
                            }
                            else SendPrivateMessage(username, "数据库达到上线，无法保存!");
                            break;
                    }
                }
                else LogDebugToConsole(username + " 已经被忽略!");
            }
        }

        /// <summary>
        /// Called on each MCC tick, around 10 times per second
        /// </summary>
        public override void Update()
        {
            DateTime dateNow = DateTime.Now;
            if (nextMailSend < dateNow)
            {
                LogDebugToConsole("正在检查邮件并发送 @ " + DateTime.Now);

                // Process at most 3 mails at a time to avoid spamming. Other mails will be processed on next mail send
                HashSet<string> onlinePlayersLowercase = new HashSet<string>(GetOnlinePlayers().Select(name => name.ToLower()));
                foreach (Mail mail in mailDatabase.Where(mail => !mail.Delivered && onlinePlayersLowercase.Contains(mail.RecipientLowercase)).Take(3))
                {
                    string sender = mail.Anonymous ? "Anonymous" : mail.Sender;
                    SendPrivateMessage(mail.Recipient, sender + " 发送信息: " + mail.Content);
                    mail.setDelivered();
                    LogDebugToConsole("已到达: " + mail.ToString());
                }

                lock (readWriteLock)
                {
                    mailDatabase.RemoveAll(mail => mail.Delivered);
                    mailDatabase.RemoveAll(mail => mail.DateSent.AddDays(Settings.Mailer_MailRetentionDays) < DateTime.Now);
                    mailDatabase.SaveToFile(Settings.Mailer_DatabaseFile);
                }

                nextMailSend = dateNow.AddSeconds(10);
            }
        }

        /// <summary>
        /// Called when the Mail Database or Ignore list has changed on disk
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileMonitorCallback(object sender, FileSystemEventArgs e)
        {
            lock (readWriteLock)
            {
                mailDatabase = MailDatabase.FromFile(Settings.Mailer_DatabaseFile);
                ignoreList = IgnoreList.FromFile(Settings.Mailer_IgnoreListFile);
            }
        }

        /// <summary>
        /// Interprets local commands.
        /// </summary>
        private string ProcessInternalCommand(string cmd, string[] args)
        {
            if (args.Length > 0)
            {
                string commandName = args[0].ToLower();
                switch (commandName)
                {
                    case "getmails":
                        return "== Mails in database ==\n" + string.Join("\n", mailDatabase);

                    case "getignored":
                        return "== Ignore list ==\n" + string.Join("\n", ignoreList);

                    case "addignored":
                    case "removeignored":
                        if (args.Length > 1 && IsValidName(args[1]))
                        {
                            string username = args[1].ToLower();
                            if (commandName == "addignored")
                            {
                                lock (readWriteLock)
                                {
                                    if (!ignoreList.Contains(username))
                                    {
                                        ignoreList.Add(username);
                                        ignoreList.SaveToFile(Settings.Mailer_IgnoreListFile);
                                    }
                                }
                                return "Added " + args[1] + " to the ignore list!";
                            }
                            else
                            {
                                lock (readWriteLock)
                                {
                                    if (ignoreList.Contains(username))
                                    {
                                        ignoreList.Remove(username);
                                        ignoreList.SaveToFile(Settings.Mailer_IgnoreListFile);
                                    }
                                }
                                return "Removed " + args[1] + " from the ignore list!";
                            }
                        }
                        else return "Missing or invalid name. Usage: " + commandName + " <username>";
                }
            }
            return "See usage: /help mailer";
        }
    }
}
