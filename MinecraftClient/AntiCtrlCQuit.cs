using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using MinecraftClient.Protocol.Session;
using MinecraftClient.Protocol;
namespace MinecraftClient
{
//反Ctrl+C退出
// MinecraftClient/AntiCtrlCQuit.cs
public class Example
{
    public static void Main()

    {
        AppDomain appd = AppDomain.CurrentDomain;
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true; //true: 不导致退出。false: 会导致退出
            ConsoleIO.WriteLineFormatted("你正在尝试按下Ctrl+C关闭程序，但被取消了操作.");
        };
    }
}
}
