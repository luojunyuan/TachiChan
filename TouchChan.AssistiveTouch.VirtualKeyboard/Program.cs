﻿using System.Diagnostics;
using System.Runtime.InteropServices;
using TouchChan.AssistiveTouch.NativeMethods;

var gameHandle = (IntPtr)int.Parse(args[0]);
var parent = Process.GetProcessById(int.Parse(args[1]));
parent.EnableRaisingEvents = true;
parent.Exited += (s, e) => Environment.Exit(0);

ComWrappers.RegisterForMarshalling(WinFormsComInterop.WinFormsComWrappers.Instance);

Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);

var ctrlButton = new Button
{
    Text = "Ctrl",
    Width = 75,
    Height = 75,
    Margin = new Padding(20, 40, 0, 0),
    BackColor = Color.White
};
ctrlButton.MouseDown += (_, args) => Simulate.Down(Simulate.KeyCode.Control);
ctrlButton.MouseUp += (_, _) => Simulate.Up(Simulate.KeyCode.Control);

var form = new ParamForm
{
    AllowTransparency = true,
    TransparencyKey = Color.Green,
    BackColor = Color.Green,
    FormBorderStyle = FormBorderStyle.None,
    TopMost = true,
    ShowInTaskbar = false
};
form.Controls.Add(ctrlButton);

var hooker = new GameWindowHookerOld(gameHandle, form.Close);
var dpi = form.CreateGraphics().DpiX / 96.0;
void SizeDelegate(object? sender, GameWindowHookerOld.WindowPosition pos)
{
    form.Height = (int)(pos.Height / dpi);
    form.Width = (int)(pos.Width / dpi);
    form.Left = (int)(pos.Left / dpi);            
    form.Top = (int)(pos.Top / dpi);
}
hooker.WindowPositionChanged += SizeDelegate;
hooker.UpdatePosition(gameHandle);
form.Closed += (_, _) =>
{
    hooker.WindowPositionChanged -= SizeDelegate;
    hooker = null;
};

Application.Run(form);

class ParamForm : Form
{
    private const int WS_EX_NOACTIVATE = 0x08000000;
    protected override CreateParams CreateParams
    {
        get
        {
            var createParams = base.CreateParams;

            createParams.ExStyle |= WS_EX_NOACTIVATE;
            return createParams;
        }
    }
}
