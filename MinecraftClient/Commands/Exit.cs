using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftClient.Commands
{
    public class Exit : Command
    {
        public override string CMDName { get { return "exit"; } }
        public override string CMDDesc { get { return "exit: 从服务器断开."; } }

        public override string Run(McClient handler, string command, Dictionary<string, object> localVars)
        {
            MCC.LogToConsole("正在关闭 MCC汉化版");
            MCC.LogToConsole("感谢您的使用:)");
            MCC.LogToConsole("再会!");
            Thread.Sleep(1000);
            Program.Exit();
            return "";
        }

        public override IEnumerable<string> getCMDAliases()
        {
            return new string[] { "quit" };
        }
    }
}
