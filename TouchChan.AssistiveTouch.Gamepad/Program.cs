using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using TouchChan.AssistiveTouch.Core.Extend;

var pipeClient = new AnonymousPipeClientStream(PipeDirection.Out, args[0]);
var parent = Process.GetProcessById(int.Parse(args[1]));
parent.EnableRaisingEvents = true;
parent.Exited += (s, e) => Environment.Exit(0);
var gameWindow = int.Parse(args[2]);
var engine = args[3];


var aegleseeker = new GameController(gameWindow);
if (!aegleseeker.IsConnected)
    return;

aegleseeker.LoadConfig(engine);
aegleseeker.Start();

using var sw = new StreamWriter(pipeClient);
sw.AutoFlush = true;
aegleseeker.OpenMenu += (_, e) => sw.WriteLine("OpenMenu");


while (GetMessage(out var msg, IntPtr.Zero, 0, 0) != false)
{
    TranslateMessage(msg);
    DispatchMessage(msg);
}

[DllImport("user32.dll")]
static extern bool GetMessage(out IntPtr lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);
[DllImport("user32.dll")]
static extern bool TranslateMessage(in IntPtr lpMsg);
[DllImport("user32.dll")]
static extern IntPtr DispatchMessage(in IntPtr lpMsg);
