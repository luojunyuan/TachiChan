using System.Text.Json;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel;
using Windows.Foundation.Collections;

namespace WinRTLibrary
{
    public sealed class Class1
    {
        public static async Task X()
        {

            {
                var connection = new AppServiceConnection
                {
                    AppServiceName = "CommunicationService",
                    PackageFamilyName = Package.Current.Id.FamilyName
                };
                await connection.OpenAsync();

                var valueSet = new ValueSet
    {
        { "result", "json" }
    };
                await connection.SendMessageAsync(valueSet);
                return;
            }
        }
    }
}