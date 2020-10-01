using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinecraftClient.Mapping;

namespace MinecraftClient.Commands
{
    public class Dig : Command
    {
        public override string CMDName { get { return "dig"; } }
        public override string CMDDesc { get { return "dig <x> <y> <z>: 挖一个方块"; } }

        public override string Run(McClient handler, string command, Dictionary<string, object> localVars)
        {
            if (!handler.GetTerrainEnabled())
                return "请先打开 Terrain and Movements.";

            if (hasArg(command))
            {
                string[] args = getArgs(command);
                if (args.Length == 3)
                {
                    try
                    {
                        int x = int.Parse(args[0]);
                        int y = int.Parse(args[1]);
                        int z = int.Parse(args[2]);
                        Location blockToBreak = new Location(x, y, z);
                        if (blockToBreak.DistanceSquared(handler.GetCurrentLocation().EyesLocation()) > 25)
                            return "你离这个方块太远了.";
                        if (handler.GetWorld().GetBlock(blockToBreak).Type == Material.Air)
                            return "那个方块是空气!";
                        if (handler.DigBlock(blockToBreak))
                            return String.Format("正在开始破坏 {0} {1} {2} 的方块", x, y, z);
                        else return "破坏失败!";
                    }
                    catch (FormatException) { return CMDDesc; }
                }
                else return CMDDesc;
            }
            else return CMDDesc;
        }
    }
}
