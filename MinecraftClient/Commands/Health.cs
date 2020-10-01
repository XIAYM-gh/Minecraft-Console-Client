using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftClient.Commands
{
    class Health : Command
    {
        public override string CMDName { get { return "health"; } }
        public override string CMDDesc { get { return "health: Display Health and Food saturation."; } }

        public override string Run(McClient handler, string command, Dictionary<string, object> localVars)
        {
            return "生命值: " + handler.GetHealth() + ", 饥饿度: " + handler.GetSaturation() + ", 等级: " + handler.GetLevel() + ", 总经验: " + handler.GetTotalExperience();
        }
    }
}
