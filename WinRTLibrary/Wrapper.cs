using System.Text.Json;

namespace WinRTLibrary
{
    public static class Wrapper
    {
        public static void Send(string message) 
        {
            new WinRTComponent.Class().AFunction(message);
        }
    }
}