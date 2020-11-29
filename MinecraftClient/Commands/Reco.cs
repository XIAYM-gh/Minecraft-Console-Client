using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftClient.Commands
{
    public class Reco : Command
    {
        public override string CMDName { get { return "reco"; } }
        public override string CMDDesc { get { return "reco [用户]: 重新连接至服务器"; } }

        public override string Run(McClient handler, string command, Dictionary<string, object> localVars)
        {
            string[] args = getArgs(command);
            if (args.Length > 0)
            {
                if (!Settings.SetAccount(args[0]))
                {
                    return "未知用户 '" + args[0] + "'.";
                }
            }
            Program.Restart();
            return "";
        }
    }
}
