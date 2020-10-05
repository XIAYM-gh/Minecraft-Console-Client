using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftClient.Commands
{
    public class Sneak : Command
    {
        private bool sneaking = false;
        public override string CMDName { get { return "Sneak"; } }
        public override string CMDDesc { get { return "Sneak: 切换潜行"; } }

        public override string Run(McClient handler, string command, Dictionary<string, object> localVars)
        {
            if (sneaking)
            {
                var result = handler.SendEntityAction(Protocol.EntityActionType.StopSneaking);
                if (result)
                    sneaking = false;
                return  result ? "你取消了蹲下" : "失败";
            }
            else
            {
                var result = handler.SendEntityAction(Protocol.EntityActionType.StartSneaking);
                if (result)
                    sneaking = true;
                return  result ? "你已蹲下" : "失败";
            }
            
        }
    }
}
