#include "pch.h"

#include <winrt/Windows.ApplicationModel.h>
#include <winrt/Windows.ApplicationModel.AppService.h>
#include <winrt/Windows.Foundation.Collections.h>

using namespace winrt;
using namespace Windows::Foundation;

using namespace winrt::Windows::Foundation;
using namespace winrt::Windows::ApplicationModel;
using namespace winrt::Windows::ApplicationModel::AppService;
using namespace winrt::Windows::Foundation::Collections;
using namespace std;
//
//int __stdcall wWinMain(HINSTANCE, HINSTANCE, PWSTR, int)
//{
//}

int main()
{
    init_apartment();
    //Uri uri(L"http://aka.ms/cppwinrt");
    //printf("1Hello, %ls!\n", uri.AbsoluteUri().c_str());

    AppServiceConnection connection;
    connection.AppServiceName(L"CommunicationService");
    connection.PackageFamilyName(Package::Current().Id().FamilyName());
    connection.OpenAsync().get();

    winrt::Windows::Foundation::Collections::ValueSet set;
    set.Insert(L"result", PropertyValue::CreateString(L"Highhhhhhhhhh"));
    connection.SendMessageAsync(set).get();
}
