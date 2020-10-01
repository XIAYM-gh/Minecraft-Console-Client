using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftClient.Commands
{
    class UseItem : Command
    {
        public override string CMDName { get { return "useitem"; } }
        public override string CMDDesc { get { return "useitem: 左键手上的物品."; } }

        public override string Run(McClient handler, string command, Dictionary<string, object> localVars)
        {
            if (handler.GetInventoryEnabled())
            {
                handler.UseItemOnHand();
                return "已左键物品.";
            }
            else return "请先启用 inventoryhandling.";
        }
    }
}
