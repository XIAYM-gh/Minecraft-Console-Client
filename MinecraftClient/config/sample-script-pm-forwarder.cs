//MCCScript 1.0

MCC.LoadBot(new PMForwarder());

//MCCScript Extensions

/// <summary>
/// This bot can forward received PMs to other players
/// </summary>
public class PMForwarder : ChatBot
{
    private const string PMRecipientsFile = "pm-forward-to.txt";
    private string[] pmRecipients;

    public PMForwarder()
    {
        pmRecipients = LoadDistinctEntriesFromFile(PMRecipientsFile);
        if (Settings.Bots_Owners.Count == 0)
            LogToConsole("在设置 INI 文件中没有机器人所有者 正在取消加载");
        else if (pmRecipients.Length == 0)
            LogToConsole("'" + PMRecipientsFile + "' 没有 PM 收件人 正在取消加载");
        else LogToConsole(String.Format(
            "从业主 {0} 转发 PMs 到收件人 {1}",
            String.Join(", ", Settings.Bots_Owners), String.Join(", ", pmRecipients)));
    }

    public override void GetText(string text)
    {
        text = GetVerbatim(text);
        string message = "", sender = "";
        if (IsPrivateMessage(text, ref message, ref sender) && Settings.Bots_Owners.Contains(sender.ToLower().Trim()))
        {
            LogToConsole("转送 PM 至 " + String.Join(", ", pmRecipients));
            foreach (string recipient in pmRecipients)
                SendPrivateMessage(recipient, message);
        }
    }
}
}
