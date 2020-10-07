public class MyTextBox:TextBox
{
public const int WM_COPY=0x301;
public const int WM_CUT=0x300;
//Retry!
WndProc(ref Message m);
protected override void WndProc(ref Message m)
{
if (e.Msg==WM_COPY||e.Msg ==WM_CUT)return;//不处理
base.WndProc(ref m);
}
}
