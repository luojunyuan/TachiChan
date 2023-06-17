// See https://aka.ms/new-console-template for more information
using System.Text;
using TinyIpc.Messaging;

if (args.Contains("-channel"))
{
    var ipcSender = new TinyMessageBus("PreferenceChannel");
    _ = ipcSender.PublishAsync(Encoding.UTF8.GetBytes("onaga sui da"));
}