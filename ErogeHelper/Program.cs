// See https://aka.ms/new-console-template for more information
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

if (args.Contains("-channel"))
{
    var connection = new AppServiceConnection();
    connection.AppServiceName = "CommunicationService";
    connection.PackageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName;//"Microsoft.AppServiceBridge_8wekyb3d8bbwe";
    await connection.OpenAsync();
    var valueSet = new ValueSet
    {
        { "response", $"{Random.Shared.Next()}" }
    };
    await connection.SendMessageAsync(valueSet);
    return;
}