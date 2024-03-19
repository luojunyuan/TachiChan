// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include <wrl/module.h>
#include <wrl/implements.h>
#include <shobjidl_core.h>
#include <wil/resource.h>
#include <Shellapi.h>
#include <Strsafe.h>
#include <string>
#include <sysinfoapi.h>
#include <vector>
#include <shlwapi.h>

using namespace Microsoft::WRL;

HMODULE g_hModule = nullptr;

BOOL APIENTRY DllMain(HMODULE hModule,
    DWORD ul_reason_for_call,
    LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        g_hModule = hModule;
        break;
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

std::wstring GetModulePath()
{
    WCHAR modulePath[MAX_PATH];
    GetModuleFileName(g_hModule, modulePath, MAX_PATH);
    std::wstring path(modulePath);
    size_t pos = path.find_last_of(L"\\");
    if (pos != std::wstring::npos)
    {
        return path.substr(0, pos + 1);
    }
    return L"";
}


class TestExplorerCommandBase : public RuntimeClass<RuntimeClassFlags<ClassicCom>, IExplorerCommand, IObjectWithSite>
{
public:
    virtual const wchar_t* Title() = 0;
    virtual const EXPCMDFLAGS Flags() { return ECF_DEFAULT; }
    virtual const EXPCMDSTATE State(_In_opt_ IShellItemArray* selection) { return ECS_ENABLED; }
    virtual const IFACEMETHODIMP InvokeMe(IShellItemArray* selection) { return S_OK; }

    // IExplorerCommand
    IFACEMETHODIMP GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name)
    {
        *name = nullptr;
        auto title = wil::make_cotaskmem_string_nothrow(Title());
        RETURN_IF_NULL_ALLOC(title);
        *name = title.release();
        return S_OK;
    }
    IFACEMETHODIMP GetIcon(_In_opt_ IShellItemArray*, _Outptr_result_nullonfailure_ PWSTR* icon) { *icon = nullptr; return E_NOTIMPL; }
    IFACEMETHODIMP GetToolTip(_In_opt_ IShellItemArray*, _Outptr_result_nullonfailure_ PWSTR* infoTip) { *infoTip = nullptr; return E_NOTIMPL; }
    IFACEMETHODIMP GetCanonicalName(_Out_ GUID* guidCommandName) { *guidCommandName = GUID_NULL;  return S_OK; }
    IFACEMETHODIMP GetState(_In_opt_ IShellItemArray* selection, _In_ BOOL okToBeSlow, _Out_ EXPCMDSTATE* cmdState)
    {
        *cmdState = State(selection);
        return S_OK;
    }
    IFACEMETHODIMP Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*) noexcept try
    {
        return InvokeMe(selection);
    }
    CATCH_RETURN();

    IFACEMETHODIMP GetFlags(_Out_ EXPCMDFLAGS* flags) { *flags = Flags(); return S_OK; }
    IFACEMETHODIMP EnumSubCommands(_COM_Outptr_ IEnumExplorerCommand** enumCommands) { *enumCommands = nullptr; return E_NOTIMPL; }

    // IObjectWithSite
    IFACEMETHODIMP SetSite(_In_ IUnknown* site) noexcept { m_site = site; return S_OK; }
    IFACEMETHODIMP GetSite(_In_ REFIID riid, _COM_Outptr_ void** site) noexcept { return m_site.CopyTo(riid, site); }

protected:
    ComPtr<IUnknown> m_site;
};

class SubExplorerCommandHandler final : public TestExplorerCommandBase
{
public:
    const wchar_t* Title() override { return L"Start"; }
    const IFACEMETHODIMP InvokeMe(IShellItemArray* selection) override
    {
        if (selection)
        {
            DWORD count;
            RETURN_IF_FAILED(selection->GetCount(&count));
            if (count > 0)
            {
                ComPtr<IShellItem> item;
                RETURN_IF_FAILED(selection->GetItemAt(0, &item));

                PWSTR filePath;
                RETURN_IF_FAILED(item->GetDisplayName(SIGDN_FILESYSPATH, &filePath));
                wil::unique_cotaskmem_string filePathCleanup(filePath);
                std::wstring quotedFilePath = L"\"" + std::wstring(filePath) + L"\"";

                std::wstring executablePath = GetModulePath() + L"TouchChan\\TouchChan.exe";

                SHELLEXECUTEINFO sei = { 0 };
                sei.cbSize = sizeof(SHELLEXECUTEINFO);
                sei.fMask = SEE_MASK_DEFAULT;
                sei.lpVerb = L"open";
                sei.lpFile = executablePath.c_str();
                sei.lpParameters = quotedFilePath.c_str();
                sei.nShow = SW_SHOWNORMAL;

                if (!ShellExecuteEx(&sei))
                {
                    return HRESULT_FROM_WIN32(GetLastError());
                }
            }
        }
        return S_OK;
    }
};

class SubExplorerCommandHandler2 final : public TestExplorerCommandBase
{
public:
    const wchar_t* Title() override { return L"Start with..."; }
    const IFACEMETHODIMP InvokeMe(IShellItemArray* selection) override
    {
        if (selection)
        {
            DWORD count;
            RETURN_IF_FAILED(selection->GetCount(&count));
            if (count > 0)
            {
                ComPtr<IShellItem> item;
                RETURN_IF_FAILED(selection->GetItemAt(0, &item));

                PWSTR filePath;
                RETURN_IF_FAILED(item->GetDisplayName(SIGDN_FILESYSPATH, &filePath));
                wil::unique_cotaskmem_string filePathCleanup(filePath);
                std::wstring quotedFilePath = L"\"" + std::wstring(filePath) + L"\"" + L" -le";

                std::wstring executablePath = GetModulePath() + L"TouchChan\\TouchChan.exe";

                SHELLEXECUTEINFO sei = { 0 };
                sei.cbSize = sizeof(SHELLEXECUTEINFO);
                sei.fMask = SEE_MASK_DEFAULT;
                sei.lpVerb = L"open";
                sei.lpFile = executablePath.c_str();
                sei.lpParameters = quotedFilePath.c_str();
                sei.nShow = SW_SHOWNORMAL;

                if (!ShellExecuteEx(&sei))
                {
                    return HRESULT_FROM_WIN32(GetLastError());
                }
            }
        }
        return S_OK;
    }
};


class EnumCommands : public RuntimeClass<RuntimeClassFlags<ClassicCom>, IEnumExplorerCommand>
{
public:
    EnumCommands()
    {
        m_commands.push_back(Make<SubExplorerCommandHandler>());
        m_commands.push_back(Make<SubExplorerCommandHandler2>());
        m_current = m_commands.cbegin();
    }

    // IEnumExplorerCommand
    IFACEMETHODIMP Next(ULONG celt, __out_ecount_part(celt, *pceltFetched) IExplorerCommand** apUICommand, __out_opt ULONG* pceltFetched)
    {
        ULONG fetched{ 0 };
        wil::assign_to_opt_param(pceltFetched, 0ul);

        for (ULONG i = 0; (i < celt) && (m_current != m_commands.cend()); i++)
        {
            m_current->CopyTo(&apUICommand[0]);
            m_current++;
            fetched++;
        }

        wil::assign_to_opt_param(pceltFetched, fetched);
        return (fetched == celt) ? S_OK : S_FALSE;
    }

    IFACEMETHODIMP Skip(ULONG /*celt*/) { return E_NOTIMPL; }
    IFACEMETHODIMP Reset()
    {
        m_current = m_commands.cbegin();
        return S_OK;
    }
    IFACEMETHODIMP Clone(__deref_out IEnumExplorerCommand** ppenum) { *ppenum = nullptr; return E_NOTIMPL; }

private:
    std::vector<ComPtr<IExplorerCommand>> m_commands;
    std::vector<ComPtr<IExplorerCommand>>::const_iterator m_current;
};

inline std::wstring get_module_folderpath(HMODULE mod = nullptr, const bool removeFilename = true)
{
    wchar_t buffer[MAX_PATH + 1];
    DWORD actual_length = GetModuleFileNameW(mod, buffer, MAX_PATH);
    if (GetLastError() == ERROR_INSUFFICIENT_BUFFER)
    {
        const DWORD long_path_length = 0xFFFF; // should be always enough
        std::wstring long_filename(long_path_length, L'\0');
        actual_length = GetModuleFileNameW(mod, const_cast<LPWSTR>(long_filename.data()), long_path_length);
        PathRemoveFileSpecW(const_cast<LPWSTR>(long_filename.data()));
        long_filename.resize(std::wcslen(long_filename.data()));
        long_filename.shrink_to_fit();
        return long_filename;
    }

    if (removeFilename)
    {
        PathRemoveFileSpecW(buffer);
    }
    return { buffer, static_cast<uint64_t>(lstrlenW(buffer)) };
}

class HelloWorldCommand : public RuntimeClass<RuntimeClassFlags<ClassicCom>, IExplorerCommand, IObjectWithSite>
{
public:
    // IExplorerCommand methods
    IFACEMETHODIMP GetTitle(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* name)
    {
        *name = nullptr;
        auto title = wil::make_cotaskmem_string_nothrow(L"TachiChan");
        RETURN_IF_NULL_ALLOC(title);
        *name = title.release();
        return S_OK;
    }

    IFACEMETHODIMP GetIcon(_In_opt_ IShellItemArray* items, _Outptr_result_nullonfailure_ PWSTR* iconPath)
    {
        std::wstring iconResourcePath = get_module_folderpath(g_hModule);
        iconResourcePath += L"\\icon.png";
        return SHStrDupW(iconResourcePath.c_str(), iconPath);
    }


    IFACEMETHODIMP GetToolTip(_In_opt_ IShellItemArray*, _Outptr_result_nullonfailure_ PWSTR* infoTip) { *infoTip = nullptr; return E_NOTIMPL; }
    IFACEMETHODIMP GetCanonicalName(_Out_ GUID* guidCommandName) { *guidCommandName = GUID_NULL;  return S_OK; }
    IFACEMETHODIMP GetState(_In_opt_ IShellItemArray* selection, _In_ BOOL okToBeSlow, _Out_ EXPCMDSTATE* cmdState)
    {
#pragma warning(disable : 4996)
        OSVERSIONINFOEX osvi = { sizeof(OSVERSIONINFOEX) };
        DWORD buildNumber = GetVersionExW(reinterpret_cast<OSVERSIONINFO*>(&osvi)) ? osvi.dwBuildNumber : 0;

        if (buildNumber >= 22000)
        {
            if (selection && okToBeSlow)
            {
                *cmdState = ECS_ENABLED;
                return S_OK;
            }
            *cmdState = ECS_HIDDEN;
        }
        else
        {
            *cmdState = ECS_ENABLED;
            return S_OK;
        }
    }

    IFACEMETHODIMP Invoke(_In_opt_ IShellItemArray* selection, _In_opt_ IBindCtx*) noexcept try
    {
        return S_OK;
    }
    CATCH_RETURN();

    IFACEMETHODIMP GetFlags(_Out_ EXPCMDFLAGS* flags) 
    { 
        *flags = ECF_HASSUBCOMMANDS;
        return S_OK;
    }
    IFACEMETHODIMP EnumSubCommands(_COM_Outptr_ IEnumExplorerCommand** enumCommands)
    { 
        *enumCommands = nullptr;
        auto e = Make<EnumCommands>();
        return e->QueryInterface(IID_PPV_ARGS(enumCommands));
    }

    // IObjectWithSite methods
    IFACEMETHODIMP SetSite(_In_ IUnknown* site) noexcept { m_site = site; return S_OK; }
    IFACEMETHODIMP GetSite(_In_ REFIID riid, _COM_Outptr_ void** site) noexcept { return m_site.CopyTo(riid, site); }

protected:
    ComPtr<IUnknown> m_site;
};


class __declspec(uuid("C7F36224-0EBF-4CE7-A07B-71BD79CFEFC7")) HelloWorldCommand1 final : public HelloWorldCommand {};

CoCreatableClass(HelloWorldCommand1)


STDAPI DllGetActivationFactory(_In_ HSTRING activatableClassId, _COM_Outptr_ IActivationFactory** factory)
{
    return Module<ModuleType::InProc>::GetModule().GetActivationFactory(activatableClassId, factory);
}

STDAPI DllCanUnloadNow()
{
    return Module<InProc>::GetModule().GetObjectCount() == 0 ? S_OK : S_FALSE;
}

STDAPI DllGetClassObject(_In_ REFCLSID rclsid, _In_ REFIID riid, _COM_Outptr_ void** instance)
{
    return Module<InProc>::GetModule().GetClassObject(rclsid, riid, instance);
}
