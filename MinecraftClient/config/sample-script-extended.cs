//MCCScript 1.0

/* This script demonstrates how to use methods and arguments */

string text = "你好";

if (args.Length > 0)
    text = args[0];
   
for (int i = 0; i < 5; i++)
{
    int count = MCC.GetVarAsInt("test") + 1;
    MCC.SetVar("test", count);
    SendHelloWorld(count, text);
    SleepBetweenSends();
}

//MCCScript Extensions

/* Here you can define methods for use into your script */

void SendHelloWorld(int count, string text)
{
    MCC.SendText("你好！世界 " + count + ": " + text);
}

void SleepBetweenSends()
{
    MCC.LogToConsole("等待 5 秒钟...");
    Thread.Sleep(5000);
}
