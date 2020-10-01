using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftClient.ChatBots
{
    /// <summary>
    /// This bot automatically re-join the server if kick message contains predefined string (Server is restarting ...)
    /// </summary>
    public class AutoRelog : ChatBot
    {
        private string[] dictionary = new string[0];
        private int attempts;
        private int delay;

        /// <summary>
        /// This bot automatically re-join the server if kick message contains predefined string
        /// </summary>
        /// <param name="DelayBeforeRelog">Delay before re-joining the server (in seconds)</param>
        /// <param name="retries">Number of retries if connection fails (-1 = infinite)</param>
        public AutoRelog(int DelayBeforeRelog, int retries)
        {
            attempts = retries;
            if (attempts == -1) { attempts = int.MaxValue; }
            McClient.ReconnectionAttemptsLeft = attempts;
            delay = DelayBeforeRelog;
            if (delay < 1) { delay = 1; }
            LogDebugToConsole("正在尝试从" + attempts + "启动..");
        }

        public override void Initialize()
        {
            McClient.ReconnectionAttemptsLeft = attempts;
            if (Settings.AutoRelog_IgnoreKickMessage)
            {
                LogDebugToConsole("初始化无信息踢出中..");
            }
            else
            {
                if (System.IO.File.Exists(Settings.AutoRelog_KickMessagesFile))
                {
                    LogDebugToConsole("已加载文件: " + System.IO.Path.GetFullPath(Settings.AutoRelog_KickMessagesFile));

                    dictionary = System.IO.File.ReadAllLines(Settings.AutoRelog_KickMessagesFile, Encoding.UTF8);

                    for (int i = 0; i < dictionary.Length; i++)
                    {
                        LogDebugToConsole("  已加载信息: " + dictionary[i]);
                        dictionary[i] = dictionary[i].ToLower();
                    }
                }
                else
                {
                    LogToConsole("文件未找到: " + System.IO.Path.GetFullPath(Settings.AutoRelog_KickMessagesFile));

                    LogDebugToConsole("  文件路径: " + System.IO.Directory.GetCurrentDirectory());
                }
            }
        }

        public override bool OnDisconnect(DisconnectReason reason, string message)
        {
            if (reason == DisconnectReason.UserLogout)
            {
                LogDebugToConsole("用户或MCC主动断开连接，忽略.");
            }
            else
            {
                message = GetVerbatim(message);
                string comp = message.ToLower();

                LogDebugToConsole("被踢出信息: " + message);

                if (Settings.AutoRelog_IgnoreKickMessage)
                {
                    LogDebugToConsole("忽略被踢消息，尝试重新连接..");
                    LogToConsole("等待 " + delay + " s后重新连接");
                    System.Threading.Thread.Sleep(delay * 1000);
                    ReconnectToTheServer();
                    return true;
                }

                foreach (string msg in dictionary)
                {
                    if (comp.Contains(msg))
                    {
                        LogDebugToConsole("Message contains '" + msg + "'. Reconnecting.");
                        LogToConsole("等待 " + delay + " s后重新连接...");
                        System.Threading.Thread.Sleep(delay * 1000);
                        McClient.ReconnectionAttemptsLeft = attempts;
                        ReconnectToTheServer();
                        return true;
                    }
                }

                LogDebugToConsole("信息没有任何关键词，忽略.");
            }

            return false;
        }

        public static bool OnDisconnectStatic(DisconnectReason reason, string message)
        {
            if (Settings.AutoRelog_Enabled)
            {
                AutoRelog bot = new AutoRelog(Settings.AutoRelog_Delay, Settings.AutoRelog_Retries);
                bot.Initialize();
                return bot.OnDisconnect(reason, message);
            }
            return false;
        }
    }
}
