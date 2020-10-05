//MCCScript 1.0

MCC.LoadBot(new SugarCaneMiner());

//MCCScript Extensions

/// <summary>
/// All saved options.
/// </summary>
[Serializable]
public class Options
{
    public string path_mail { get; set; }
    public string path_setting { get; set; }
    public string botname { get; set; }
    public int interval_sendmail { get; set; }
    public int maxSavedMails { get; set; }
    public int maxSavedMails_Player { get; set; }
    public int daysTosaveMsg { get; set; }
    public bool debug_msg { get; set; }
    public string[] moderator = new string[0];
    public DateTime lastReset { get; set; }
    public int timevar_100ms { get; set; }

    public Options()
    {
        path_mail = AppDomain.CurrentDomain.BaseDirectory + "mails.txt";          // Path where the mail file is saved. You can also apply a normal path like @"C:\Users\SampleUser\Desktop"
        path_setting = AppDomain.CurrentDomain.BaseDirectory + "options.txt";     // Path where the settings are saved
        interval_sendmail = 100;                                                    // Intervall atempting to send mails / do a respawn [in 100 ms] -> eg. 100 * 100ms = 10 sec
        maxSavedMails = 2000;                                                       // How many mails you want to safe
        maxSavedMails_Player = 3;                                                   // How many mails can be sent per player
        daysTosaveMsg = 30;                                                         // After how many days the message should get deleted
        debug_msg = true;                                                           // Disable debug Messages for a cleaner console

        timevar_100ms = 0;
        lastReset = DateTime.UtcNow;
    }
}

/// <summary>
/// The Way every mail is safed.
/// </summary>
[Serializable]
public class Message
{
    string sender;
    string destination;
    string content;
    DateTime timestamp;
    bool delivered;

    public Message(string sender_1, string destination_1, string content_1)
    {
        sender = sender_1;
        destination = destination_1;
        content = content_1;
        timestamp = DateTime.UtcNow;
        delivered = false;
    }

    // Obtain Message data.
    public string GetSender()
    {
        return sender;
    }
    public string GetDestination()
    {
        return destination;
    }
    public string GetContent()
    {
        return content;
    }
    public DateTime GetTimeStamp()
    {
        return timestamp;
    }
    public bool isDelivered()
    {
        return delivered;
    }

    // Set the message to "delivered" to clear the list later.
    public void setDelivered()
    {
        delivered = true;
    }
}

public class Mail : ChatBot
{
    Message[] logged_msg;
    Options options;

    /// <summary>
    ///  Sets the message an option cache
    /// </summary>
    public override void Initialize()
    {
        logged_msg = new Message[0];
        options = new Options();
    }

    /// <summary>
    /// Standard settings for the bot.
    /// </summary>
    public override void AfterGameJoined()
    {
        if (!File.Exists(options.path_setting))
        {
            SaveOptionsToFile();
        }
        else
        {
            GetOptionsFromFile();
        }

        if (!File.Exists(options.path_mail))
        {
            SaveMailsToFile();
        }

        deleteOldMails();
        options.lastReset = DateTime.UtcNow;
        options.botname = GetUsername();
    }

    /// <summary>
    /// Timer for autorespawn and the message deliverer
    /// </summary>
    public override void Update()
    {
        if (options.timevar_100ms == options.interval_sendmail)
        {
            DeliverMail();
            PerformInternalCommand("respawn");

            if ((DateTime.Now - options.lastReset).TotalDays > options.daysTosaveMsg)
            {
                deleteOldMails();
                options.lastReset = DateTime.UtcNow;
            }

            options.timevar_100ms = 0;
        }
        options.timevar_100ms++;
    }

    /// <summary>
    /// Listening for Messages.
    /// </summary>
    public override void GetText(string text)
    {
        string message = "";
        string username = "";

        text = GetVerbatim(text);

        if (IsPrivateMessage(text, ref message, ref username))
        {
            if(username.ToLower() != options.botname.ToLower())
            {
                message = message.ToLower();
                cmd_reader(message, username, isMessageFromMod(username));
            }
        }

    }

    /// <summary>
    /// Interprets command.
    /// </summary>
    public void cmd_reader(string message, string sender, bool isMod)
    {
        if (message.Contains("sendmail"))
        {
            GetMailsFromFile();
            if (getSentMessagesByUser(sender) < options.maxSavedMails_Player && logged_msg.Length < options.maxSavedMails)
            {


                string content = "";
                string destination = "";
                bool destination_ended = false;

                for (int i = message.IndexOf("sendmail") + 9; i < message.Length; i++) // + 4 -> get first letter of the name.
                {
                    if (message[i] != Convert.ToChar(" ") && !destination_ended)
                    {
                        destination += message[i]; // extract destination
                    }
                    else
                    {
                        destination_ended = true;

                        content += message[i];  // extract message content
                    }
                }

                if (IsValidName(sender) && IsValidName(destination) && content != string.Empty)
                {
                    AddMail(sender, destination, content);
                }
                else
                {
                    SendPrivateMessage(sender, "发生了一些错误!");
                }
            }
            else
            {
                SendPrivateMessage(sender, "不能保存信息 被限制了!");
            }
            clearLogged_msg();
        }

        if (isMod || options.moderator.Length == 0) // Delete the safe file of the bot to reset all mods || 2. otion to get the owner as a moderator.
        {
            mod_Commands(message, sender);
        }
    }

    private void mod_Commands(string message, string sender)
    {

        // These commands can be exploited easily and may cause huge damage.

        if (message.Contains("getmails"))
        {
            if (options.debug_msg)
            {
                LogToConsole("正在列出所有消息 \n 由: " + sender);
            }
            GetMailsFromFile();
            foreach (Message m in logged_msg)
            {
                LogToConsole(m.GetSender() + " " + m.GetDestination() + " " + m.GetContent() + " " + Convert.ToString(m.GetTimeStamp()));
            }

            clearLogged_msg();
        }

        /*
         // Only uncomment for testing reasons 

        if (message.Contains("clearmails"))
        {
            if (debug_msg)
            {
                LogToConsole("Clearing Messages.");
            }
            clearSavedMails();
        }
        */

        if (message.Contains("respawn"))
        {
            if (options.debug_msg)
            {
                LogToConsole("正在重生! \n 由: " + sender);
            }
            PerformInternalCommand("respawn");
        }

        if (message.Contains("deliver"))
        {
            if (options.debug_msg)
            {
                LogToConsole("正在发送邮件! \n 由: " + sender);
            }
            DeliverMail();
        }

        if (message.Contains("reconnect"))
        {
            if (options.debug_msg)
            {
                LogToConsole("正在重新连接! \n 由: " + sender);
            }
            SaveMailsToFile();
            SaveOptionsToFile();
            ReconnectToTheServer();
        }

        if (message.Contains("deleteoldmails"))
        {
            if (options.debug_msg)
            {
                LogToConsole("正在删除旧邮件! \n 由: " + sender);
            }
            deleteOldMails();
            SaveMailsToFile();
        }

        if (message.Contains("addmod"))
        {
            string name = "";

            for (int i = message.IndexOf("addMod") + 7; i < message.Length; i++)
            {
                if (message[i] != Convert.ToChar(" "))
                {
                    name += message[i];
                }
                else
                {
                    break;
                }
            }

            addMod(name);
            SendPrivateMessage(sender, name + "现在是版主");
            SaveOptionsToFile();

            if (options.debug_msg)
            {
                LogToConsole("已添加 " + name + " 于版主! \n 由: " + sender);
            }
        }

        if (message.Contains("removemod"))
        {
            string name = "";

            for (int i = message.IndexOf("addMod") + 7; i < message.Length; i++)
            {
                if (message[i] != Convert.ToChar(" "))
                {
                    name += message[i];
                }
                else
                {
                    break;
                }
            }

            removeMod(name);
            SendPrivateMessage(sender, name + "现在不是版主");
            SaveOptionsToFile();

            if (options.debug_msg)
            {
                LogToConsole("已移除 " + name + " 为版主! \n 由: " + sender);
            }
        }

        //////////////////////////////
        // Change options through mc chat
        //////////////////////////////

        if (message.Contains("toggledebug"))
        {
            if (options.debug_msg)
            {
                options.debug_msg = false;
                if (options.debug_msg)
                {
                    LogToConsole(sender + ": 关闭控制台注销!");
                }
            }
            else
            {
                options.debug_msg = true;
                if (options.debug_msg)
                {
                    LogToConsole(sender + ": 关闭控制台注销!");
                }
            }
            SendPrivateMessage(sender, "设置改变! 加入申请!");
            SaveOptionsToFile();
        }

        if (message.Contains("daystosavemsg"))
        {
            options.daysTosaveMsg = getIntInCommand(message, "daystosavemsg");
            SaveOptionsToFile();
            SendPrivateMessage(sender, "设置改变! 加入申请!");

            if (options.debug_msg)
            {
                LogToConsole(sender + " 修改 daystosavemsg 为: " + Convert.ToString(options.daysTosaveMsg));
            }
        }

        if (message.Contains("intervalsendmail"))
        {
            options.interval_sendmail = getIntInCommand(message, "intervalsendmail");
            SaveOptionsToFile();
            SendPrivateMessage(sender, "设置改变! 加入申请!");

            if (options.debug_msg)
            {
                LogToConsole(sender + " 修改 intervalsendmail 为: " + Convert.ToString(options.interval_sendmail));
            }
        }

        if (message.Contains("maxsavedmails"))
        {
            options.maxSavedMails = getIntInCommand(message, "maxsavedmails");
            SaveOptionsToFile();
            SendPrivateMessage(sender, "设置改变! 加入申请!");

            if (options.debug_msg)
            {
                LogToConsole(sender + " 修改 maxsavedmails 为: " + Convert.ToString(options.maxSavedMails));
            }
        }

        if (message.Contains("maxmailsperplayer"))
        {
            options.maxSavedMails_Player = getIntInCommand(message, "maxmailsperplayer");
            SaveOptionsToFile();
            SendPrivateMessage(sender, "设置改变! 加入申请!");

            if (options.debug_msg)
            {
                LogToConsole(sender + " 修改 maxmailsperplayer 为: " + Convert.ToString(options.maxSavedMails_Player));
            }
        }

        if (message.Contains("changemailpath"))
        {
            string path = "";
            for (int i = message.IndexOf("changemailpath") + "changemailpath".Length + 1; i < message.Length; i++)
            {
                if (message[i] != Convert.ToChar(" "))
                {
                    path += message[i];
                }
                else
                {
                    break;
                }
            }
            options.path_mail = AppDomain.CurrentDomain.BaseDirectory + path;
            SendPrivateMessage(sender, "设置改变!");
            SaveOptionsToFile();
            GetOptionsFromFile();

            if (options.debug_msg)
            {
                LogToConsole(sender + " 修改 mailpath 为: " + Convert.ToString(options.path_mail));
            }

        }

        if (message.Contains("changesettingspath"))
        {
            string path = "";
            for (int i = message.IndexOf("changesettingspath") + "changesettingspath".Length + 1; i < message.Length; i++)
            {
                if (message[i] != Convert.ToChar(" "))
                {
                    path += message[i];
                }
                else
                {
                    break;
                }
            }
            options.path_setting = AppDomain.CurrentDomain.BaseDirectory + path;
            SendPrivateMessage(sender, "设置改变!");
            SaveOptionsToFile();
            GetOptionsFromFile();

            if(options.debug_msg)
            {
                LogToConsole(sender + " 修改 settingsspath 为: " + Convert.ToString(options.path_setting));
            }

        }

        if (message.Contains("listsettings"))
        {
            SendPrivateMessage(sender, "debugmsg: " + Convert.ToString(options.debug_msg) + "; daystosavemsg: " + Convert.ToString(options.daysTosaveMsg) + "; intervalsendmail: " + Convert.ToString(options.interval_sendmail) + "; maxsavedmails: " + Convert.ToString(options.maxSavedMails) + "; maxsavedmails_player: " + Convert.ToString(options.maxSavedMails_Player + "; messagepath: " + options.path_mail + "; settingspath: " + options.path_setting));
        }
    }

    /// <summary>
    /// Get the number after a certain word in the message.
    /// </summary>
    public int getIntInCommand(string message, string searched)
    {
        string num = "";
        for (int i = message.IndexOf(searched) + searched.Length + 1; i < message.Length; i++)
        {
            if (message[i] != Convert.ToChar(" "))
            {
                num += message[i];
            }
            else
            {
                return Int32.Parse(num);
            }
        }
        return Int32.Parse(num);
    }

    /// <summary>
    /// Clear the safe File.
    /// </summary>
    public void clearSavedMails()
    {
        clearLogged_msg();
        SaveMailsToFile();
    }

    /// <summary>
    /// Clear the messages in ram.
    /// </summary>
    public void clearLogged_msg()
    {
        logged_msg = new Message[0];
    }

    /// <summary>
    /// Add a player who can moderate the bot.
    /// </summary>
    public void addMod(string name)
    {
        string[] temp = options.moderator;
        options.moderator = new string[options.moderator.Length + 1];

        for (int i = 0; i < temp.Length; i++)
        {
            options.moderator[i] = temp[i];
        }
        options.moderator[options.moderator.Length - 1] = name;
    }

    /// <summary>
    /// Remove a player from the moderator list.
    /// </summary>
    public void removeMod(string name)
    {

        for (int i = 0; i < options.moderator.Length; i++)
        {
            if (options.moderator[i] == name)
            {
                options.moderator[i] = string.Empty;
            }
        }
        options.moderator = options.moderator.Where(x => !string.IsNullOrEmpty(x)).ToArray();
    }

    /// <summary>
    /// Serialize mails to binary file.
    /// </summary>
    public void SaveMailsToFile()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(options.path_mail, FileMode.Create, FileAccess.Write);

        formatter.Serialize(stream, logged_msg);
        stream.Close();

        if (options.debug_msg)
        {
            LogToConsole("已保存邮件文件!"  + " 位置: " + options.path_mail + " 时间: " + Convert.ToString(DateTime.UtcNow));
        }
    }

    /// <summary>
    /// Get mails from save file.
    /// </summary>
    public void GetMailsFromFile()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(options.path_mail, FileMode.Open, FileAccess.Read);

        logged_msg = (Message[])formatter.Deserialize(stream);
        stream.Close();

        if (options.debug_msg)
        {
            LogToConsole("正在从文件加载邮件!" + " 位置: " + options.path_mail + " 时间: " + Convert.ToString(DateTime.UtcNow));
        }
    }

    /// <summary>
    /// Serialize settings to binary file.
    /// </summary>
    public void SaveOptionsToFile()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(options.path_setting, FileMode.Create, FileAccess.Write);

        formatter.Serialize(stream, options);
        stream.Close();

        if (options.debug_msg)
        {
            LogToConsole("已保存邮件文件! " + "位置: " + options.path_setting + " 时间: " + Convert.ToString(DateTime.UtcNow));
        }
    }

    /// <summary>
    /// Get settings from save file.
    /// </summary>
    public void GetOptionsFromFile()
    {

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(options.path_setting, FileMode.Open, FileAccess.Read);

        options = (Options)formatter.Deserialize(stream);
        stream.Close();

        if (options.debug_msg)
        {
            LogToConsole("正在从文件加载邮件! " + "位置: " + options.path_setting + " 时间: " + Convert.ToString(DateTime.UtcNow));
        }
    }

    /// <summary>
    /// Add a message to the list.
    /// </summary>
    public void AddMail(string sender, string destination, string content)
    {
        GetMailsFromFile();

        Message[] tmp = logged_msg;
        logged_msg = new Message[logged_msg.Length + 1];

        for (int i = 0; i < tmp.Length; i++)
        {
            logged_msg[i] = tmp[i];
        }

        logged_msg[logged_msg.Length - 1] = new Message(sender, destination, content);

        SaveMailsToFile();
        SendPrivateMessage(sender, "信息已保存!");
    }

    /// <summary>
    /// Try to send all messages.
    /// </summary>
    public void DeliverMail()
    {
        LogToConsole("正在寻找要发送的邮件: " + DateTime.UtcNow); // Can not be disabled to indicate, that the script is still running. 
        GetMailsFromFile();

        foreach(string Player in GetOnlinePlayers())
        {
            foreach (Message msg in logged_msg)
            {
                if (Player.ToLower() == msg.GetDestination().ToLower() && !msg.isDelivered())
                {
                    SendPrivateMessage(msg.GetDestination(), msg.GetSender() + " mailed: " + msg.GetContent());
                    msg.setDelivered();

                    if (options.debug_msg)
                    {
                        //原句：LogToConsole("Message of " + msg.GetSender() + " delivered to " + msg.GetDestination() + ".");
                        LogToConsole(msg.GetSender() + " 的信息已传递给 " + msg.GetDestination());
                    }
                }
            }
        }

        logged_msg = logged_msg.Where(x => !x.isDelivered()).ToArray();
        SaveMailsToFile();
        clearLogged_msg();
    }

    /// <summary>
    /// See how many messages of a user are saved.
    /// </summary>
    public int getSentMessagesByUser(string player)
    {
        int mailcount = 0;

        foreach (Message msg in logged_msg)
        {
            if (msg.GetSender() == player)
            {
                mailcount++;
            }
        }
        return mailcount;
    }

    /// <summary>
    /// Test if the sender is in the moderator list.
    /// </summary>
    public bool isMessageFromMod(string player)
    {
        foreach (string mod in options.moderator)
        {
            if (mod.ToLower() == player.ToLower())
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Deleting mails older than a month.
    /// </summary>
    public void deleteOldMails()
    {
        GetMailsFromFile();

        for(int i = 0; i < logged_msg.Length; i++)
        {
            if ((DateTime.UtcNow - logged_msg[i].GetTimeStamp()).TotalDays > options.daysTosaveMsg)
            {
                logged_msg[i].setDelivered();
            }
        }
        logged_msg = logged_msg.Where(x => !x.isDelivered()).ToArray();
        SaveMailsToFile();
        clearLogged_msg();
    }
}
