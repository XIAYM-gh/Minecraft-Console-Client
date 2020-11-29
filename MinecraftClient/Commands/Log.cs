using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftClient.Commands
{
    public class Log : Command
    {
        public override string CMDName { get { return "log"; } }
        public override string CMDDesc { get { return "log <text>: 执行脚本时在屏幕上显示文字."; } }

        public override string Run(McClient handler, string command, Dictionary<string, object> localVars)
        {
            if (hasArg(command))
            {
                ConsoleIO.WriteLogLine(getArg(command));
                return "";
            }
            else return CMDDesc;
        }
    }
}
