using System.Text.Json;

namespace WinRTLibrary
{
    public static class Class1
    {
        public static void Send(string message) 
        {
            new WinRTComponent.Class().AFunction(message);
        }
    }
}