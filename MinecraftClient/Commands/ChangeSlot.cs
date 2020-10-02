using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftClient.Commands
{
    class ChangeSlot : Command
    {
        public override string CMDName { get { return "changeslot"; } }
        public override string CMDDesc { get { return "changeslot <1-9>: 切换物品栏"; } }

        public override string Run(McClient handler, string command, Dictionary<string, object> localVars)
        {
            if (!handler.GetInventoryEnabled())
                return "请先在配置文件中开启 InventoryHandling";

            if (hasArg(command))
            {
                short slot;
                try
                {
                    slot = Convert.ToInt16(getArg(command));
                }
                catch (FormatException)
                {
                    return "这不是一个有效的数字.";
                }
                if (slot >= 1 && slot <= 9)
                {
                    if (handler.ChangeSlot(slot-=1))
                    {
                        return "切换至栏 " + (slot+=1);
                    }
                    else
                    {
                        return "不能切换栏";
                    }
                }
            }
            return CMDDesc;
        }
    }
}
