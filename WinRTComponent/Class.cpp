#include "pch.h"
#include "Class.h"
#include "Class.g.cpp"

#include <winrt/Windows.ApplicationModel.h>
#include <winrt/Windows.ApplicationModel.AppService.h>
#include <winrt/Windows.Foundation.Collections.h>

using namespace winrt::Windows::Foundation;
using namespace winrt::Windows::ApplicationModel;
using namespace winrt::Windows::ApplicationModel::AppService;
using namespace winrt::Windows::Foundation::Collections;
using namespace std;

namespace winrt::WinRTComponent::implementation
{
	void Class::AFunction(hstring value)
	{	
		//AppServiceConnection connection{ nullptr };
		AppServiceConnection connection;
		connection.AppServiceName(L"CommunicationService");
		connection.PackageFamilyName(Package::Current().Id().FamilyName());

		connection.OpenAsync();

		winrt::Windows::Foundation::Collections::ValueSet set;
		set.Insert(L"result", PropertyValue::CreateString(L"High"));
		connection.SendMessageAsync(set);
	}

}
