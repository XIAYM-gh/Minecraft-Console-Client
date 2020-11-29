using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftClient.Commands
{
    public class Send : Command
    {
        public override string CMDName { get { return "send"; } }
        public override string CMDDesc { get { return "send <text>: 发送文本或命令."; } }

        public override string Run(McClient handler, string command, Dictionary<string, object> localVars)
        {
            if (hasArg(command))
            {
                handler.SendText(getArg(command));
                return "";
            }
            else return CMDDesc;
        }
    }
}
