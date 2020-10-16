using System;
using System.Threading;
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
            Console.WriteLine("You have Press Ctrl+C");
        };
    }
}
