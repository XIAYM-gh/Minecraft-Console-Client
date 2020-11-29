using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftClient.Commands
{
    public class Set : Command
    {
        public override string CMDName { get { return "set"; } }
        public override string CMDDesc { get { return "set varname=value: 设置一个变量."; } }

        public override string Run(McClient handler, string command, Dictionary<string, object> localVars)
        {
            if (hasArg(command))
            {
                string[] temp = getArg(command).Split('=');
                if (temp.Length > 1)
                {
                    if (Settings.SetVar(temp[0], getArg(command).Substring(temp[0].Length + 1)))
                    {
                        return ""; //Success
                    }
                    else return "变量名必须为大小写字母和数字.";
                }
                else return CMDDesc;
            }
            else return CMDDesc;
        }
    }
}
