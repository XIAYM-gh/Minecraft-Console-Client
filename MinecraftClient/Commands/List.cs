using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftClient.Commands
{
    public class List : Command
    {
        public override string CMDName { get { return "list"; } }
        public override string CMDDesc { get { return "list: 查询玩家列表."; } }

        public override string Run(McClient handler, string command, Dictionary<string, object> localVars)
        {
            return "玩家列表: " + String.Join(", ", handler.GetOnlinePlayers());
        }
    }
}

