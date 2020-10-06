using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using MinecraftClient.Protocol.Session;
using MinecraftClient.Protocol;

namespace MinecraftClient
{
    /// <summary>
    /// Contains main settings for Minecraft Console Client
    /// Allows settings loading from an INI file
    /// </summary>

    public static class Settings
    {
        //Minecraft Console Client client information used for BrandInfo setting
        private const string MCCBrandInfo = "Minecraft-Console-Client/" + Program.Version;

        //Main Settings.
        //Login: Username or email adress used as login for Minecraft/Mojang account
        //Username: The actual username of the user, obtained after login to the account
        public static string Login = "";
        public static string Username = "";
        public static string Password = "";
        public static string ServerIP = "";
        public static ushort ServerPort = 25565;
        public static string ServerVersion = "";
        public static bool ServerMayHaveForge = true;
        public static string SingleCommand = "";
        public static string ConsoleTitle = "";

        //Proxy Settings
        public static bool ProxyEnabledLogin = false;
        public static bool ProxyEnabledIngame = false;
        public static string ProxyHost = "";
        public static int ProxyPort = 0;
        public static Proxy.ProxyHandler.Type proxyType = Proxy.ProxyHandler.Type.HTTP;
        public static string ProxyUsername = "";
        public static string ProxyPassword = "";

        //Minecraft Settings
        public static bool MCSettings_Enabled = true;
        public static string MCSettings_Locale = "zh_CN";
        public static byte MCSettings_Difficulty = 0;
        public static byte MCSettings_RenderDistance = 8;
        public static byte MCSettings_ChatMode = 0;
        public static bool MCSettings_ChatColors = true;
        public static byte MCSettings_MainHand = 0;
        public static bool MCSettings_Skin_Hat = true;
        public static bool MCSettings_Skin_Cape = true;
        public static bool MCSettings_Skin_Jacket = false;
        public static bool MCSettings_Skin_Sleeve_Left = false;
        public static bool MCSettings_Skin_Sleeve_Right = false;
        public static bool MCSettings_Skin_Pants_Left = false;
        public static bool MCSettings_Skin_Pants_Right = false;
        public static byte MCSettings_Skin_All
        {
            get
            {
                return (byte)(
                      ((MCSettings_Skin_Cape ? 1 : 0) << 0)
                    | ((MCSettings_Skin_Jacket ? 1 : 0) << 1)
                    | ((MCSettings_Skin_Sleeve_Left ? 1 : 0) << 2)
                    | ((MCSettings_Skin_Sleeve_Right ? 1 : 0) << 3)
                    | ((MCSettings_Skin_Pants_Left ? 1 : 0) << 4)
                    | ((MCSettings_Skin_Pants_Right ? 1 : 0) << 5)
                    | ((MCSettings_Skin_Hat ? 1 : 0) << 6)
                );
            }
        }

        //Other Settings
        public static string TranslationsFile_FromMCDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.minecraft\assets\objects\eb\ebf762c137bd91ab2496397f2504e250f3c5d1ba";
        public static string TranslationsFile_Website_Index = "https://launchermeta.mojang.com/v1/packages/bdb68de96a44ec1e9ed6d9cfcd2ee973be618c3a/1.16.json";
        public static string TranslationsFile_Website_Download = "http://resources.download.minecraft.net";
        public static TimeSpan splitMessageDelay = TimeSpan.FromSeconds(2);
        public static List<string> Bots_Owners = new List<string>();
        public static TimeSpan botMessageDelay = TimeSpan.FromSeconds(2);
        public static string Language = "zh_CN";
        public static bool interactiveMode = true;
        public static char internalCmdChar = '/';
        public static bool playerHeadAsIcon = false;
        public static string chatbotLogFile = "";
        public static bool CacheScripts = true;
        public static string BrandInfo = MCCBrandInfo;
        public static bool DisplaySystemMessages = true;
        public static bool DisplayXPBarMessages = true;
        public static bool DisplayChatLinks = true;
        public static bool TerrainAndMovements = false;
        public static bool InventoryHandling = false;
        public static string PrivateMsgsCmdName = "tell";
        public static CacheType SessionCaching = CacheType.Disk;
        public static bool DebugMessages = false;
        public static bool ResolveSrvRecords = true;
        public static bool ResolveSrvRecordsShortTimeout = true;
        public static bool EntityHandling = false;
        public static bool AutoRespawn = false;

        //AntiAFK Settings
        public static bool AntiAFK_Enabled = false;
        public static int AntiAFK_Delay = 600;
        public static string AntiAFK_Command = "/ping";

        //Hangman Settings
        public static bool Hangman_Enabled = false;
        public static bool Hangman_English = true;
        public static string Hangman_FileWords_EN = "hangman-en.txt";
        public static string Hangman_FileWords_FR = "hangman-fr.txt";

        //Alerts Settings
        public static bool Alerts_Enabled = false;
        public static bool Alerts_Beep_Enabled = true;
        public static string Alerts_MatchesFile = "alerts.txt";
        public static string Alerts_ExcludesFile = "alerts-exclude.txt";

        //ChatLog Settings
        public static bool ChatLog_Enabled = false;
        public static bool ChatLog_DateTime = true;
        public static string ChatLog_File = "chatlog.txt";
        public static ChatBots.ChatLog.MessageFilter ChatLog_Filter = ChatBots.ChatLog.MessageFilter.AllMessages;

        //PlayerListLog Settings
        public static bool PlayerLog_Enabled = false;
        public static string PlayerLog_File = "playerlog.txt";
        public static int PlayerLog_Delay = 600;

        //AutoRelog Settings
        public static bool AutoRelog_Enabled = false;
        public static int AutoRelog_Delay = 10;
        public static int AutoRelog_Retries = 3;
        public static bool AutoRelog_IgnoreKickMessage = false;
        public static string AutoRelog_KickMessagesFile = "kickmessages.txt";

        //Script Scheduler Settings
        public static bool ScriptScheduler_Enabled = false;
        public static string ScriptScheduler_TasksFile = "tasks.ini";

        //Remote Control
        public static bool RemoteCtrl_Enabled = false;
        public static bool RemoteCtrl_AutoTpaccept = true;
        public static bool RemoteCtrl_AutoTpaccept_Everyone = false;

        //Chat Message Parsing
        public static bool ChatFormat_Builtins = true;
        public static Regex ChatFormat_Public = null;
        public static Regex ChatFormat_Private = null;
        public static Regex ChatFormat_TeleportRequest = null;

        //Auto Respond
        public static bool AutoRespond_Enabled = false;
        public static string AutoRespond_Matches = "matches.ini";

        //Auto Attack
        public static bool AutoAttack_Enabled = false;
        public static string AutoAttack_Mode = "single";
        public static string AutoAttack_Priority = "distance";

        //Auto Fishing
        public static bool AutoFishing_Enabled = false;
        public static bool AutoFishing_Antidespawn = false;

        //Auto Eating
        public static bool AutoEat_Enabled = false;
        public static int AutoEat_hungerThreshold = 6;

        //AutoCraft
        public static bool AutoCraft_Enabled = false;
        public static string AutoCraft_configFile = @"autocraft\config.ini";
        
        //Mailer
        public static bool Mailer_Enabled = false;
        public static string Mailer_DatabaseFile = "MailerDatabase.ini";
        public static string Mailer_IgnoreListFile = "MailerIgnoreList.ini";
        public static bool Mailer_PublicInteractions = false;
        public static int Mailer_MaxMailsPerPlayer = 10;
        public static int Mailer_MaxDatabaseSize = 10000;
        public static int Mailer_MailRetentionDays = 30;

        //AutoDrop
        public static bool AutoDrop_Enabled = false;
        public static string AutoDrop_Mode = "include";
        public static string AutoDrop_items = "";

        // Replay Mod
        public static bool ReplayMod_Enabled = false;
        public static int ReplayMod_BackupInterval = 3000;


        //Custom app variables and Minecraft accounts
        private static readonly Dictionary<string, object> AppVars = new Dictionary<string, object>();
        private static readonly Dictionary<string, KeyValuePair<string, string>> Accounts = new Dictionary<string, KeyValuePair<string, string>>();
        private static readonly Dictionary<string, KeyValuePair<string, ushort>> Servers = new Dictionary<string, KeyValuePair<string, ushort>>();


        private enum ParseMode { Default, Main, AppVars, Proxy, MCSettings, AntiAFK, Hangman, Alerts, ChatLog, AutoRelog, ScriptScheduler, RemoteControl, ChatFormat, AutoRespond, AutoAttack, AutoFishing, AutoEat, AutoCraft, AutoDrop, Mailer, ReplayMod };


        /// <summary>
        /// Load settings from the give INI file
        /// </summary>
        /// <param name="settingsfile">File to load</param>
        public static void LoadSettings(string settingsfile)
        {
            ConsoleIO.WriteLogLine(String.Format("正在读取设置 文件路径: {0}", System.IO.Path.GetFullPath(settingsfile)));
            if (File.Exists(settingsfile))
            {
                try
                {
                    string serverAlias = "";
                    string[] Lines = File.ReadAllLines(settingsfile);
                    ParseMode pMode = ParseMode.Default;
                    foreach (string lineRAW in Lines)
                    {
                        string line = pMode == ParseMode.Main && lineRAW.ToLower().Trim().StartsWith("password")
                            ? lineRAW.Trim() //Do not strip # in passwords
                            : lineRAW.Split('#')[0].Trim();

                        if (line.Length > 0)
                        {
                            if (line[0] == '[' && line[line.Length - 1] == ']')
                            {
                                switch (line.Substring(1, line.Length - 2).ToLower())
                                {
                                    case "alerts": pMode = ParseMode.Alerts; break;
                                    case "antiafk": pMode = ParseMode.AntiAFK; break;
                                    case "autorelog": pMode = ParseMode.AutoRelog; break;
                                    case "chatlog": pMode = ParseMode.ChatLog; break;
                                    case "hangman": pMode = ParseMode.Hangman; break;
                                    case "main": pMode = ParseMode.Main; break;
                                    case "mcsettings": pMode = ParseMode.MCSettings; break;
                                    case "scriptscheduler": pMode = ParseMode.ScriptScheduler; break;
                                    case "remotecontrol": pMode = ParseMode.RemoteControl; break;
                                    case "proxy": pMode = ParseMode.Proxy; break;
                                    case "appvars": pMode = ParseMode.AppVars; break;
                                    case "autorespond": pMode = ParseMode.AutoRespond; break;
                                    case "chatformat": pMode = ParseMode.ChatFormat; break;
                                    case "autoattack": pMode = ParseMode.AutoAttack; break;
                                    case "autofishing": pMode = ParseMode.AutoFishing; break;
                                    case "autoeat": pMode = ParseMode.AutoEat; break;
                                    case "autocraft": pMode = ParseMode.AutoCraft; break;
                                    case "mailer": pMode = ParseMode.Mailer; break;
                                    case "autodrop": pMode = ParseMode.AutoDrop; break;
                                    case "replaymod": pMode = ParseMode.ReplayMod; break;

                                    default: pMode = ParseMode.Default; break;
                                }
                            }
                            else
                            {
                                string argName = line.Split('=')[0];
                                if (line.Length > (argName.Length + 1))
                                {
                                    string argValue = line.Substring(argName.Length + 1);
                                    switch (pMode)
                                    {
                                        case ParseMode.Main:
                                            switch (argName.ToLower())
                                            {
                                                case "login": Login = argValue; break;
                                                case "password": Password = argValue; break;
                                                case "serverip": if (!SetServerIP(argValue)) serverAlias = argValue; ; break;
                                                case "singlecommand": SingleCommand = argValue; break;
                                                case "language": Language = argValue; break;
                                                case "consoletitle": ConsoleTitle = argValue; break;
                                                case "timestamps": ConsoleIO.EnableTimestamps = str2bool(argValue); break;
                                                case "exitonfailure": interactiveMode = !str2bool(argValue); break;
                                                case "playerheadicon": playerHeadAsIcon = str2bool(argValue); break;
                                                case "chatbotlogfile": chatbotLogFile = argValue; break;
                                                case "mcversion": ServerVersion = argValue; break;
                                                case "mcforge": ServerMayHaveForge = argValue.ToLower() == "auto" || str2bool(argValue); break;
                                                case "splitmessagedelay": splitMessageDelay = TimeSpan.FromSeconds(str2int(argValue)); break;
                                                case "scriptcache": CacheScripts = str2bool(argValue); break;
                                                case "showsystemmessages": DisplaySystemMessages = str2bool(argValue); break;
                                                case "showxpbarmessages": DisplayXPBarMessages = str2bool(argValue); break;
                                                case "showchatlinks": DisplayChatLinks = str2bool(argValue); break;
                                                case "terrainandmovements": TerrainAndMovements = str2bool(argValue); break;
                                                case "entityhandling": EntityHandling = str2bool(argValue); break;
                                                case "enableentityhandling": EntityHandling = str2bool(argValue); break;
                                                case "inventoryhandling": InventoryHandling = str2bool(argValue); break;
                                                case "privatemsgscmdname": PrivateMsgsCmdName = argValue.ToLower().Trim(); break;
                                                case "botmessagedelay": botMessageDelay = TimeSpan.FromSeconds(str2int(argValue)); break;
                                                case "debugmessages": DebugMessages = str2bool(argValue); break;
                                                case "autorespawn": AutoRespawn = str2bool(argValue); break;

                                                case "botowners":
                                                    Bots_Owners.Clear();
                                                    string[] names = argValue.ToLower().Split(',');
                                                    if (!argValue.Contains(",") && argValue.ToLower().EndsWith(".txt") && File.Exists(argValue))
                                                        names = File.ReadAllLines(argValue);
                                                    foreach (string name in names)
                                                        if (!String.IsNullOrWhiteSpace(name))
                                                            Bots_Owners.Add(name.Trim());
                                                    break;

                                                case "internalcmdchar":
                                                    switch (argValue.ToLower())
                                                    {
                                                        case "none": internalCmdChar = ' '; break;
                                                        case "slash": internalCmdChar = '/'; break;
                                                        case "backslash": internalCmdChar = '\\'; break;
                                                    }
                                                    break;

                                                case "sessioncache":
                                                    if (argValue == "none") { SessionCaching = CacheType.None; }
                                                    else if (argValue == "memory") { SessionCaching = CacheType.Memory; }
                                                    else if (argValue == "disk") { SessionCaching = CacheType.Disk; }
                                                    break;

                                                case "accountlist":
                                                    if (File.Exists(argValue))
                                                    {
                                                        foreach (string account_line in File.ReadAllLines(argValue))
                                                        {
                                                            //Each line contains account data: 'Alias,Login,Password'
                                                            string[] account_data = account_line.Split('#')[0].Trim().Split(',');
                                                            if (account_data.Length == 3)
                                                                Accounts[account_data[0].ToLower()]
                                                                    = new KeyValuePair<string, string>(account_data[1], account_data[2]);
                                                        }

                                                        //Try user value against aliases after load
                                                        Settings.SetAccount(Login);
                                                    }
                                                    break;

                                                case "serverlist":
                                                    if (File.Exists(argValue))
                                                    {
                                                        //Backup current server info
                                                        string server_host_temp = ServerIP;
                                                        ushort server_port_temp = ServerPort;

                                                        foreach (string server_line in File.ReadAllLines(argValue))
                                                        {
                                                            //Each line contains server data: 'Alias,Host:Port'
                                                            string[] server_data = server_line.Split('#')[0].Trim().Split(',');
                                                            server_data[0] = server_data[0].ToLower();
                                                            if (server_data.Length == 2
                                                                && server_data[0] != "localhost"
                                                                && !server_data[0].Contains('.')
                                                                && SetServerIP(server_data[1]))
                                                                Servers[server_data[0]]
                                                                    = new KeyValuePair<string, ushort>(ServerIP, ServerPort);
                                                        }

                                                        //Restore current server info
                                                        ServerIP = server_host_temp;
                                                        ServerPort = server_port_temp;

                                                        //Try server value against aliases after load
                                                        SetServerIP(serverAlias);
                                                    }
                                                    break;

                                                case "brandinfo":
                                                    switch (argValue.Trim().ToLower())
                                                    {
                                                        case "mcc": BrandInfo = MCCBrandInfo; break;
                                                        case "vanilla": BrandInfo = "vanilla"; break;
                                                        default: BrandInfo = null; break;
                                                    }
                                                    break;

                                                case "resolvesrvrecords":
                                                    if (argValue.Trim().ToLower() == "fast")
                                                    {
                                                        ResolveSrvRecords = true;
                                                        ResolveSrvRecordsShortTimeout = true;
                                                    }
                                                    else
                                                    {
                                                        ResolveSrvRecords = str2bool(argValue);
                                                        ResolveSrvRecordsShortTimeout = false;
                                                    }
                                                    break;
                                            }
                                            break;

                                        case ParseMode.Alerts:
                                            switch (argName.ToLower())
                                            {
                                                case "enabled": Alerts_Enabled = str2bool(argValue); break;
                                                case "alertsfile": Alerts_MatchesFile = argValue; break;
                                                case "excludesfile": Alerts_ExcludesFile = argValue; break;
                                                case "beeponalert": Alerts_Beep_Enabled = str2bool(argValue); break;
                                            }
                                            break;

                                        case ParseMode.AntiAFK:
                                            switch (argName.ToLower())
                                            {
                                                case "enabled": AntiAFK_Enabled = str2bool(argValue); break;
                                                case "delay": AntiAFK_Delay = str2int(argValue); break;
                                                case "command": AntiAFK_Command = argValue == "" ? "/ping" : argValue; break;
                                            }
                                            break;

                                        case ParseMode.AutoRelog:
                                            switch (argName.ToLower())
                                            {
                                                case "enabled": AutoRelog_Enabled = str2bool(argValue); break;
                                                case "delay": AutoRelog_Delay = str2int(argValue); break;
                                                case "retries": AutoRelog_Retries = str2int(argValue); break;
                                                case "ignorekickmessage": AutoRelog_IgnoreKickMessage = str2bool(argValue); break;
                                                case "kickmessagesfile": AutoRelog_KickMessagesFile = argValue; break;
                                            }
                                            break;

                                        case ParseMode.ChatLog:
                                            switch (argName.ToLower())
                                            {
                                                case "enabled": ChatLog_Enabled = str2bool(argValue); break;
                                                case "timestamps": ChatLog_DateTime = str2bool(argValue); break;
                                                case "filter": ChatLog_Filter = ChatBots.ChatLog.str2filter(argValue); break;
                                                case "logfile": ChatLog_File = argValue; break;
                                            }
                                            break;

                                        case ParseMode.Hangman:
                                            switch (argName.ToLower())
                                            {
                                                case "enabled": Hangman_Enabled = str2bool(argValue); break;
                                                case "english": Hangman_English = str2bool(argValue); break;
                                                case "wordsfile": Hangman_FileWords_EN = argValue; break;
                                                case "fichiermots": Hangman_FileWords_FR = argValue; break;
                                            }
                                            break;

                                        case ParseMode.ScriptScheduler:
                                            switch (argName.ToLower())
                                            {
                                                case "enabled": ScriptScheduler_Enabled = str2bool(argValue); break;
                                                case "tasksfile": ScriptScheduler_TasksFile = argValue; break;
                                            }
                                            break;

                                        case ParseMode.RemoteControl:
                                            switch (argName.ToLower())
                                            {
                                                case "enabled": RemoteCtrl_Enabled = str2bool(argValue); break;
                                                case "autotpaccept": RemoteCtrl_AutoTpaccept = str2bool(argValue); break;
                                                case "tpaccepteveryone": RemoteCtrl_AutoTpaccept_Everyone = str2bool(argValue); break;
                                            }
                                            break;

                                        case ParseMode.ChatFormat:
                                            switch (argName.ToLower())
                                            {
                                                case "builtins": ChatFormat_Builtins = str2bool(argValue); break;
                                                case "public": ChatFormat_Public = new Regex(argValue); break;
                                                case "private": ChatFormat_Private = new Regex(argValue); break;
                                                case "tprequest": ChatFormat_TeleportRequest = new Regex(argValue); break;
                                            }
                                            break;

                                        case ParseMode.Proxy:
                                            switch (argName.ToLower())
                                            {
                                                case "enabled":
                                                    ProxyEnabledLogin = ProxyEnabledIngame = str2bool(argValue);
                                                    if (argValue.Trim().ToLower() == "login")
                                                        ProxyEnabledLogin = true;
                                                    break;
                                                case "type":
                                                    argValue = argValue.ToLower();
                                                    if (argValue == "http") { proxyType = Proxy.ProxyHandler.Type.HTTP; }
                                                    else if (argValue == "socks4") { proxyType = Proxy.ProxyHandler.Type.SOCKS4; }
                                                    else if (argValue == "socks4a") { proxyType = Proxy.ProxyHandler.Type.SOCKS4a; }
                                                    else if (argValue == "socks5") { proxyType = Proxy.ProxyHandler.Type.SOCKS5; }
                                                    break;
                                                case "server":
                                                    string[] host_splitted = argValue.Split(':');
                                                    if (host_splitted.Length == 1)
                                                    {
                                                        ProxyHost = host_splitted[0];
                                                        ProxyPort = 80;
                                                    }
                                                    else if (host_splitted.Length == 2)
                                                    {
                                                        ProxyHost = host_splitted[0];
                                                        ProxyPort = str2int(host_splitted[1]);
                                                    }
                                                    break;
                                                case "username": ProxyUsername = argValue; break;
                                                case "password": ProxyPassword = argValue; break;
                                            }
                                            break;

                                        case ParseMode.AppVars:
                                            SetVar(argName, argValue);
                                            break;

                                        case ParseMode.AutoRespond:
                                            switch (argName.ToLower())
                                            {
                                                case "enabled": AutoRespond_Enabled = str2bool(argValue); break;
                                                case "matchesfile": AutoRespond_Matches = argValue; break;
                                            }
                                            break;
                                        case ParseMode.AutoAttack:
                                            switch (argName.ToLower())
                                            {
                                                case "enabled": AutoAttack_Enabled = str2bool(argValue); break;
                                                case "mode": AutoAttack_Mode = argValue.ToLower(); break;
                                                case "priority": AutoAttack_Priority = argValue.ToLower(); break;
                                            }
                                            break;

                                        case ParseMode.AutoFishing:
                                            switch (argName.ToLower())
                                            {
                                                case "enabled": AutoFishing_Enabled = str2bool(argValue); break;
                                                case "antidespawn": AutoFishing_Antidespawn = str2bool(argValue); break;
                                            }
                                            break;

                                        case ParseMode.AutoEat:
                                            switch (argName.ToLower())
                                            {
                                                case "enabled": AutoEat_Enabled = str2bool(argValue); break;
                                                case "threshold": AutoEat_hungerThreshold = str2int(argValue); break;
                                            }
                                            break;

                                        case ParseMode.AutoCraft:
                                            switch (argName.ToLower())
                                            {
                                                case "enabled": AutoCraft_Enabled = str2bool(argValue); break;
                                                case "configfile": AutoCraft_configFile = argValue; break;
                                            }
                                            break;

                                        case ParseMode.AutoDrop:
                                            switch (argName.ToLower())
                                            {
                                                case "enabled": AutoDrop_Enabled = str2bool(argValue); break;
                                                case "mode": AutoDrop_Mode = argValue; break;
                                                case "items": AutoDrop_items = argValue; break;
                                            }
                                            break;

                                        case ParseMode.MCSettings:
                                            switch (argName.ToLower())
                                            {
                                                case "enabled": MCSettings_Enabled = str2bool(argValue); break;
                                                case "locale": MCSettings_Locale = argValue; break;
                                                case "difficulty":
                                                    switch (argValue.ToLower())
                                                    {
                                                        case "peaceful": MCSettings_Difficulty = 0; break;
                                                        case "easy": MCSettings_Difficulty = 1; break;
                                                        case "normal": MCSettings_Difficulty = 2; break;
                                                        case "difficult": MCSettings_Difficulty = 3; break;
                                                    }
                                                    break;
                                                case "renderdistance":
                                                    MCSettings_RenderDistance = 2;
                                                    if (argValue.All(Char.IsDigit))
                                                    {
                                                        MCSettings_RenderDistance = (byte)str2int(argValue);
                                                    }
                                                    else
                                                    {
                                                        switch (argValue.ToLower())
                                                        {
                                                            case "tiny": MCSettings_RenderDistance = 2; break;
                                                            case "short": MCSettings_RenderDistance = 4; break;
                                                            case "medium": MCSettings_RenderDistance = 8; break;
                                                            case "far": MCSettings_RenderDistance = 16; break;
                                                        }
                                                    }
                                                    break;
                                                case "chatmode":
                                                    switch (argValue.ToLower())
                                                    {
                                                        case "enabled": MCSettings_ChatMode = 0; break;
                                                        case "commands": MCSettings_ChatMode = 1; break;
                                                        case "disabled": MCSettings_ChatMode = 2; break;
                                                    }
                                                    break;
                                                case "chatcolors": MCSettings_ChatColors = str2bool(argValue); break;
                                                case "skin_cape": MCSettings_Skin_Cape = str2bool(argValue); break;
                                                case "skin_jacket": MCSettings_Skin_Jacket = str2bool(argValue); break;
                                                case "skin_sleeve_left": MCSettings_Skin_Sleeve_Left = str2bool(argValue); break;
                                                case "skin_sleeve_right": MCSettings_Skin_Sleeve_Right = str2bool(argValue); break;
                                                case "skin_pants_left": MCSettings_Skin_Pants_Left = str2bool(argValue); break;
                                                case "skin_pants_right": MCSettings_Skin_Pants_Right = str2bool(argValue); break;
                                                case "skin_hat": MCSettings_Skin_Hat = str2bool(argValue); break;
                                                case "main_hand":
                                                    switch (argValue.ToLower())
                                                    {
                                                        case "left": MCSettings_MainHand = 0; break;
                                                        case "right": MCSettings_MainHand = 1; break;
                                                    }
                                                    break;
                                            }
                                            break;

                                        case ParseMode.Mailer:
                                            switch (argName.ToLower())
                                            {
                                                case "enabled": Mailer_Enabled = str2bool(argValue); break;
                                                case "database": Mailer_DatabaseFile = argValue; break;
                                                case "ignorelist": Mailer_IgnoreListFile = argValue; break;
                                                case "publicinteractions": Mailer_PublicInteractions = str2bool(argValue); break;
                                                case "maxmailsperplayer": Mailer_MaxMailsPerPlayer = str2int(argValue); break;
                                                case "maxdatabasesize": Mailer_MaxDatabaseSize = str2int(argValue); break;
                                                case "retentiondays": Mailer_MailRetentionDays = str2int(argValue); break;
                                            }
                                            break;
                                        case ParseMode.ReplayMod:
                                            switch (argName.ToLower())
                                            {
                                                case "enabled": ReplayMod_Enabled = str2bool(argValue); break;
                                                case "backupinterval": ReplayMod_BackupInterval = str2int(argValue); break;
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (IOException) { }
            }
        }

        /// <summary>
        /// Write an INI file with default settings
        /// </summary>
        /// <param name="settingsfile">File to (over)write</param>
        public static void WriteDefaultSettings(string settingsfile)
        {
            System.IO.File.WriteAllText(settingsfile, "# Minecraft Console Client v" + Program.Version + "\r\n"
                + "# Startup Config File\r\n"
                + "\r\n"
                + "[Main]\r\n"
                + "\r\n"
                + "# MCC主设置\r\n"
                + "# 留下空格在MCC启动时手动输入\r\n"
                + "# 在password一行使用 \"-\" 来离线登陆\r\n"
                + "\r\n"
                + "login=\r\n"
                + "password=\r\n"
                + "serverip=\r\n"
                + "\r\n"
                + "# 高级设置\r\n"
                + "\r\n"
                + "language=en_GB\r\n"
                + "consoletitle=%username%@%serverip% - Minecraft Console Client    #标题\r\n"
                + "internalcmdchar=slash              # 使用 'none', 'slash' 或者 'backslash'(鬼知道什么作用)\r\n"
                + "splitmessagedelay=2                # 长消息分割发送秒数(约为100字符+)\r\n"
                + "botowners=Player1,Player2,Player3  # 设置机器人的主人(使用英文逗号分割，在使用match(AutoRespond)功能时可能有用.)\r\n"
                + "botmessagedelay=2                  # 每条消息之间的间隔秒数，防止刷屏\r\n"
                + "mcversion=auto                     # 游戏版本 使用 'auto(自动匹配版本)' 或者 '1.X.X(手动设置版本，在连接禁Ping的服务器时有用)'\r\n"
                + "mcforge=auto                       # 是否使用forge 使用 'auto' 或者 'false'\r\n"
                + "brandinfo=mcc                      # 使用 'mcc','vanilla', 或者 'none'\r\n"
                + "chatbotlogfile=                    # 留空不保存日志文件\r\n"
                + "privatemsgscmdname=tell            # 一些功能使用私聊时使用的命令 / 由Remotebot机器人使用(机翻)\r\n"
                + "showsystemmessages=true            # 显示来自服务器的系统消息(例如/me,/say等)\r\n"
                + "showxpbarmessages=true             # 显示在动作栏上的消息(1.12+ ActionBar)\r\n"
                + "showchatlinks=true                 # 显示聊天信息里的链接\r\n"
                + "terrainandmovements=true           # 让你可以移动 (测试，世界交互，可能会使用更多的CPU,内存等)\r\n"
                + "inventoryhandling=false            # 切换 Inventory handling (测试，物品栏交互，可以移动物品栏)\r\n"
                + "entityhandling=false               # 切换 Entity handling (测试，实体交互)\r\n"
                + "sessioncache=disk                  # 如何保存Session(正版使用). 使用 'none(不保存)', 'memory(内存条中，重启电脑丢失)' 或者 'disk(硬盘，永久，直到Session过期)'\r\n"
                + "resolvesrvrecords=fast             # 快速Ping服务器 使用 'false(关闭)', 'fast(启用&快速)' (5s 超时), 或者 'true(启用)'. 某些服务器可能需要.\r\n"
                + "accountlist=accounts.txt           # 详见Github..\r\n"
                + "serverlist=servers.txt             # 详见Github..\r\n"
                + "playerheadicon=true                # 获取玩家头颅当做应用图标(可能在 Windows7 上不起作用)\r\n"
                + "exitonfailure=false                # 禁用错误暂停，以便在非交互式脚本中使用MCC\r\n"
                + "debugmessages=false                # 显示高级信息(请在反馈bug前打开此项)\r\n"
                + "scriptcache=true                   # 在下一次执行同样的脚本时速度会变快\r\n"
                + "timestamps=false                   # 计时发送信息(类似于1.16增加的聊天间隔功能)\r\n"
                + "autorespawn=false                  # 玩家死亡后自动重生(请确保重生点是安全的，并且可以像外挂一样直接无需等待重生，可能会被反作弊检测)\r\n"
                + "\r\n"
                + "[AppVars]\r\n"
                + "# yourvar=yourvalue\r\n"
                + "# 你可以在其他文件里用这个 比如 %var%\r\n"
                + "# %username% 和 %serverip% 是内置的参数.\r\n"
                + "\r\n"
                + "[Proxy]                            # 代理\r\n"
                + "enabled=false                      # 使用 'false', 'true', 或者 'login' 仅登录\r\n"
                + "type=HTTP                          # 支持种类: HTTP, SOCKS4, SOCKS4a, SOCKS5\r\n"
                + "server=0.0.0.0:0000                # 必须为HTTPS类型且开启443端口\r\n"
                + "username=                          # 只支持密码保护的代理\r\n"
                + "password=                          # 只支持密码保护的代理\r\n"
                + "\r\n"
                + "[ChatFormat]\r\n"
                + "# 如果你要自定义请别忘了删除 # 符号\r\n"
                + "builtins=true                      # MCC built-in 模块支持自定义聊天样式\r\n"
                + "# public=^<([a-zA-Z0-9_]+)> (.+)$\r\n"
                + "# private=^([a-zA-Z0-9_]+) 悄悄的对你说: (.+)$\r\n"
                + "# tprequest=^([a-zA-Z0-9_]+) 请求(?:你|他)传送到(?:他|你)(?:那里|这里)\\.$\r\n"
                + "# 中文版tprequest如有错误请自行修改! 太tm费劲了(bushi)!"
                + "# tprequest=^([a-zA-Z0-9_]+) has requested (?:to|that you) teleport to (?:you|them)\\.$\r\n"
                + "\r\n"
                + "[MCSettings]\r\n"
                + "enabled=true                       # 是否启用  如果禁用，将不会发送到服务器.\r\n"
                + "locale=zh_CN                       # 可选择任何在我的世界中支持的语言 zh_CN / zh_TW / en_US\r\n"
                + "renderdistance=medium              # 使用 tiny(超 近), short(近), medium(中), far(远), 或区块数值 [0 - 255,服务器默认最大限制:16]\r\n"
                + "difficulty=normal                  # 使用 MC 1.7- 难度的英文. peaceful(和平), easy(简单), normal(普通), difficult(困难)\r\n"
                + "chatmode=enabled                   # 可使用enabled无限制,commands让你只能执行命令,disable无法执行任何东西\r\n"
                + "chatcolors=true                    # 允许聊天颜色\r\n"
                + "main_hand=right                     # MC 1.9+ 主手是左或右,原本配置文件为left(左手，举盾的那个)，根据实际情况已改为right\r\n"
                + "skin_cape=true\r\n"
                + "skin_hat=true\r\n"
                + "skin_jacket=false\r\n"
                + "skin_sleeve_left=false\r\n"
                + "skin_sleeve_right=false\r\n"
                + "skin_pants_left=false\r\n"
                + "skin_pants_right=false"
                + "\r\n"
                + "# Bot Settings\r\n"
                + "\r\n"
                + "[Alerts] #警告功能\r\n"
                + "enabled=false\r\n"
                + "alertsfile=alerts.txt\r\n"
                + "excludesfile=alerts-exclude.txt\r\n"
                + "beeponalert=true\r\n"
                + "\r\n"
                + "[AntiAFK] #反挂机\r\n"
                + "enabled=false\r\n"
                + "delay=600 #10 = 1s\r\n"
                + "command=/ping\r\n"
                + "\r\n"
                + "[AutoRelog] #自动执行(非自动回复)\r\n"
                + "enabled=false\r\n"
                + "delay=10\r\n"
                + "retries=3 #-1 = unlimited\r\n"
                + "ignorekickmessage=false\r\n"
                + "kickmessagesfile=kickmessages.txt\r\n"
                + "\r\n"
                + "[ChatLog] #保存聊天日志\r\n"
                + "enabled=false\r\n"
                + "timestamps=true\r\n"
                + "filter=messages\r\n"
                + "logfile=chatlog-%username%-%serverip%.txt		#聊天格式 username为用户名，serverip为服务器ip\r\n"
                + "\r\n"
                + "[Hangman] #未知功能，请提交issuse! (https://github.com/XIAYM-gh/Minecraft-Console-Client-ChineseTranslate/)\r\n"
                + "enabled=false\r\n"
                + "english=true\r\n"
                + "wordsfile=hangman-en.txt\r\n"
                + "fichiermots=hangman-fr.txt\r\n"
                + "\r\n"
                + "[ScriptScheduler] #自动执行脚本 请见用户使用手册(https://github.com/XIAYM-gh/Minecraft-Console-Client-ChineseTranslate/)\r\n"
                + "enabled=false\r\n"
                + "tasksfile=tasks.ini\r\n"
                + "\r\n"
                + "[RemoteControl] #自动接受传送请求\r\n"
                + "enabled=false\r\n"
                + "autotpaccept=true\r\n"
                + "tpaccepteveryone=false\r\n"
                + "\r\n"
                + "[AutoRespond] #自动回复 MCC最np的功能\r\n"
                + "enabled=false\r\n"
                + "matchesfile=matches.ini\r\n"
                + "\r\n"
                + "[AutoAttack] #自动攻击(小心反作弊)\r\n"
                + "# Entity Handling 需要打开\r\n"
                + "enabled=false\r\n"
                + "mode=single                        # single 或 multi. \r\n"
                + "priority=distance                  # health 或者 distance. 只会在single在模式时激活\r\n"
                + "\r\n"
                + "[AutoFishing] #自动钓鱼(小心管理)\r\n"
                + "# Entity Handling 需要打开\r\n"
                + "enabled=false\r\n"
                + "antidespawn=false\r\n"
                + "\r\n"
                + "[AutoEat] #自动吃东西(秒吃,行为像挂端一样)\r\n"
                + "# Inventory Handling 需要打开\r\n"
                + "enabled=false\r\n"
                + "threshold=6\r\n"
                + "\r\n"
                + "[AutoCraft] #自动合成 MCC最复杂的功能\r\n"
                + "# Inventory Handling 需要打开\r\n"
                + "# 如果需要使用熔炉，请打开 terrainandmovements\r\n"
                + "enabled=false\r\n"
                + "configfile=autocraft\\config.ini\r\n"
                + "\r\n"
                + "[Mailer]\r\n"
                + "# 让机器人看起来像一个邮箱插件!\r\n"
                + "enabled=false\r\n"
                + "database=MailerDatabase.ini\r\n"
                + "ignorelist=MailerIgnoreList.ini\r\n"
                + "publicinteractions=false\r\n"
                + "maxmailsperplayer=10\r\n"
                + "maxdatabasesize=10000\r\n"
                + "retentiondays=30\r\n"
                + "\r\n"
                + "[AutoDrop] #自动丢弃,配合 AutoCraft 变成工具人!\r\n"
                + "# Inventory Handling 需要打开\r\n"
                + "enabled=false\r\n"
                + "mode=include                      # include, exclude 或 everything. \r\n"
                + "items=                            # 用英文逗号分割 \r\n"
                + "# 物品列表: \r\n"
                + "# https://github.com/XIAYM-gh/Minecraft-Console-Client-ChineseTranslate/blob/master/MinecraftClient/Inventory/ItemType.cs \r\n"
                + "\r\n"
                + "[ReplayMod]\r\n"
                + "# 使用Replaymod且保存录像!(稳定差,且容易崩溃)\r\n"
                + "# 不使用/quit命令退出将不会保存录像.\r\n"
                + "enabled=false\r\n"
                + "backupinterval=300                # 多长时间后会自动保存录像，设置为-1为从不\r\n"
                + "汉化 By XIAYM.\r\n", Encoding.UTF8);
        }

        /// <summary>
        /// Convert the specified string to an integer, defaulting to zero if invalid argument
        /// </summary>
        /// <param name="str">String to parse as an integer</param>
        /// <returns>Integer value</returns>
        public static int str2int(string str)
        {
            try
            {
                return Convert.ToInt32(str.Trim());
            }
            catch {
                ConsoleIO.WriteLogLine("配置文件 '" + str + "' 转换失败. 请检查你的设置");
                return 0;
            }
        }

        /// <summary>
        /// Convert the specified string to a boolean value, defaulting to false if invalid argument
        /// </summary>
        /// <param name="str">String to parse as a boolean</param>
        /// <returns>Boolean value</returns>
        public static bool str2bool(string str)
        {
            if (String.IsNullOrEmpty(str))
                return false;
            str = str.Trim().ToLowerInvariant();
            return str == "true" || str == "1";
        }

        /// <summary>
        /// Load login/password using an account alias
        /// </summary>
        /// <returns>True if the account was found and loaded</returns>
        public static bool SetAccount(string accountAlias)
        {
            accountAlias = accountAlias.ToLower();
            if (Accounts.ContainsKey(accountAlias))
            {
                Settings.Login = Accounts[accountAlias].Key;
                Settings.Password = Accounts[accountAlias].Value;
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Load server information in ServerIP and ServerPort variables from a "serverip:port" couple or server alias
        /// </summary>
        /// <returns>True if the server IP was valid and loaded, false otherwise</returns>
        public static bool SetServerIP(string server)
        {
            server = server.ToLower();
            string[] sip = server.Split(':');
            string host = sip[0];
            ushort port = 25565;

            if (sip.Length > 1)
            {
                try
                {
                    port = Convert.ToUInt16(sip[1]);
                }
                catch (FormatException) { return false; }
            }

            if (host == "localhost" || host.Contains('.'))
            {
                //Server IP (IP or domain names contains at least a dot)
                if (sip.Length == 1 && host.Contains('.') && host.Any(c => char.IsLetter(c)) && ResolveSrvRecords)
                    //Domain name without port may need Minecraft SRV Record lookup
                    ProtocolHandler.MinecraftServiceLookup(ref host, ref port);
                ServerIP = host;
                ServerPort = port;
                return true;
            }
            else if (Servers.ContainsKey(server))
            {
                //Server Alias (if no dot then treat the server as an alias)
                ServerIP = Servers[server].Key;
                ServerPort = Servers[server].Value;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Set a custom %variable% which will be available through expandVars()
        /// </summary>
        /// <param name="varName">Name of the variable</param>
        /// <param name="varData">Value of the variable</param>
        /// <returns>True if the parameters were valid</returns>
        public static bool SetVar(string varName, object varData)
        {
            lock (AppVars)
            {
                varName = new string(varName.TakeWhile(char.IsLetterOrDigit).ToArray()).ToLower();
                if (varName.Length > 0)
                {
                    AppVars[varName] = varData;
                    return true;
                }
                else return false;
            }
        }

        /// <summary>
        /// Get a custom %variable% or null if the variable does not exist
        /// </summary>
        /// <param name="varName">Variable name</param>
        /// <returns>The value or null if the variable does not exists</returns>
        public static object GetVar(string varName)
        {
            if (AppVars.ContainsKey(varName))
                return AppVars[varName];
            return null;
        }

        /// <summary>
        /// Replace %variables% with their value from global AppVars
        /// </summary>
        /// <param name="str">String to parse</param>
        /// <param name="localContext">Optional local variables overriding global variables</param>
        /// <returns>Modifier string</returns>
        public static string ExpandVars(string str, Dictionary<string, object> localVars = null)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '%')
                {
                    bool varname_ok = false;
                    StringBuilder var_name = new StringBuilder();

                    for (int j = i + 1; j < str.Length; j++)
                    {
                        if (!char.IsLetterOrDigit(str[j]) && str[j] != '_')
                        {
                            if (str[j] == '%')
                                varname_ok = var_name.Length > 0;
                            break;
                        }
                        else var_name.Append(str[j]);
                    }

                    if (varname_ok)
                    {
                        string varname = var_name.ToString();
                        string varname_lower = varname.ToLower();
                        i = i + varname.Length + 1;

                        switch (varname_lower)
                        {
                            case "username": result.Append(Username); break;
                            case "login": result.Append(Login); break;
                            case "serverip": result.Append(ServerIP); break;
                            case "serverport": result.Append(ServerPort); break;
                            default:
                                if (localVars != null && localVars.ContainsKey(varname_lower))
                                {
                                    result.Append(localVars[varname_lower].ToString());
                                }
                                else if (AppVars.ContainsKey(varname_lower))
                                {
                                    result.Append(AppVars[varname_lower].ToString());
                                }
                                else result.Append("%" + varname + '%');
                                break;
                        }
                    }
                    else result.Append(str[i]);
                }
                else result.Append(str[i]);
            }
            return result.ToString();
        }
    }
}
