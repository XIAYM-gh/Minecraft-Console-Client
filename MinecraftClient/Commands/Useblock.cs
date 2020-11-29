using MinecraftClient.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftClient.Commands
{
    class Useblock : Command
    {
        public override string CMDName { get { return "useblock"; } }
        public override string CMDDesc { get { return "useblock <x> <y> <z>: 放置方块"; } }

        public override string Run(McClient handler, string command, Dictionary<string, object> localVars)
        {
            if (!handler.GetTerrainEnabled()) return "请先启用 TerrainHandling.";
            if (hasArg(command))
            {
                string[] args = getArgs(command);
                if (args.Length >= 3)
                {
                    int x = Convert.ToInt32(args[0]);
                    int y = Convert.ToInt32(args[1]);
                    int z = Convert.ToInt32(args[2]);
                    handler.PlaceBlock(new Location(x, y, z), Direction.Down);
                }
                else { return CMDDesc;  }
            }
            return CMDDesc;
        }
    }
}
