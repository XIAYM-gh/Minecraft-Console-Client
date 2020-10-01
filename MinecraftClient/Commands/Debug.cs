using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftClient.Commands
{
    public class Debug : Command
    {
        public override string CMDName { get { return "debug"; } }
        public override string CMDDesc { get { return "debug [on|off]: 切换调试消息."; } }

        public override string Run(McClient handler, string command, Dictionary<string, object> localVars)
        {
            if (hasArg(command))
            {
                Settings.DebugMessages = (getArg(command).ToLower() == "on");
            }
            else Settings.DebugMessages = !Settings.DebugMessages;
            return "调试消息 " + (Settings.DebugMessages ? "开启" : "关闭");
        }
    }
}
