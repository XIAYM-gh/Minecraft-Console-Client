using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftClient.Commands
{
    public class Respawn : Command
    {
        public override string CMDName { get { return "respawn"; } }
        public override string CMDDesc { get { return "respawn: 如果你死了，你可以用这个指令重生.."; } }

        public override string Run(McClient handler, string command, Dictionary<string, object> localVars)
        {
            handler.SendRespawnPacket();
            return "您已重生.";
        }
    }
}
