using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftClient.Commands
{
    public class Connect : Command
    {
        public override string CMDName { get { return "connect"; } }
        public override string CMDDesc { get { return "connect <服务器> : 连接一个存在于servers.txt中的服务器"; } }

        public override string Run(McClient handler, string command, Dictionary<string, object> localVars)
        {
            if (hasArg(command))
            {
                string[] args = getArgs(command);
                if (args.Length > 1)
                {
                    if (!Settings.SetAccount(args[1]))
                    {
                        return "未知用户 '" + args[1] + "'";
                    }
                }

                if (Settings.SetServerIP(args[0]))
                {
                    Program.Restart();
                    return "";
                }
                else return "未知服务器 IP '" + args[0] + "'";
            }
            else return CMDDesc;
        }
    }
}
