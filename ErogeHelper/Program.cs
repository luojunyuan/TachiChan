// See https://aka.ms/new-console-template for more information
using ErogeHelper;
using System.Text.Json;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

if (args.Contains("-channel"))
{
    var connection = new AppServiceConnection
    {
        AppServiceName = "CommunicationService",
        PackageFamilyName = Package.Current.Id.FamilyName
    };
    await connection.OpenAsync();

    var result = FilterProcessService.Filter();
    var jsonString = JsonSerializer.Serialize(result, typeof(IEnumerable<ProcessDataModel>), SourceGenerationContext.Default);
    var valueSet = new ValueSet
    {
        { "result", jsonString }
    };
    await connection.SendMessageAsync(valueSet);
    return;
}

