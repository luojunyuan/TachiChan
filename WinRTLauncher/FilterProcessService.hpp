#pragma comment(lib, "version.lib")
#include "pch.h"

#include <windows.h>
#include <shellapi.h>
#include <string>
#include <vector>
#include <psapi.h>
#include <winver.h>

#include <objidl.h>
#include <gdiplus.h>
using namespace Gdiplus;
#pragma comment (lib,"Gdiplus.lib")
#pragma comment(lib, "Shell32.lib")


struct ProcessDataModel {
    DWORD pid;
    std::wstring title;
    std::wstring describe;
    std::wstring path;
    std::vector<BYTE> icon;
};

const std::wstring UwpAppsTag = L"WindowsApps";
const std::wstring WindowsPath = L"C:\\Windows\\";
const std::wstring WindowsPathUpperCase = L"C:\\WINDOWS\\";

class FilterProcessService {
public:
    static std::vector<ProcessDataModel> Filter() {
        std::vector<ProcessDataModel> result;

        auto processes = GetProcessesWithWindows();
        for (const auto& process : processes) {


        }

        return result;
    }

    static BOOL CALLBACK EnumWindowsWithTitleProc(HWND hwnd, LPARAM lParam) {
        DWORD processId;
        GetWindowThreadProcessId(hwnd, &processId);

        LPWSTR windowTitle = new wchar_t[256];
        GetWindowText(hwnd, windowTitle, 256);

        if (IsWindowVisible(hwnd) && wcslen(windowTitle) > 0) {
            std::vector<std::pair<DWORD, std::wstring>>* processes = reinterpret_cast<std::vector<std::pair<DWORD, std::wstring>>*>(lParam);
            std::wstring windowTitleStr(windowTitle);
            processes->push_back(std::make_pair(processId, windowTitleStr));
        }

        return TRUE;
    }

    static std::vector<ProcessDataModel> GetProcessesWithWindows() {

        std::vector<std::pair<DWORD, std::wstring>> processes;
        EnumWindows(EnumWindowsWithTitleProc, reinterpret_cast<LPARAM>(&processes));

        std::vector<ProcessDataModel> processesExcept;
        for (int i = 0; i < processes.size(); i++)
        {
            auto pid = processes[i].first;
            auto title = processes[i].second;

            HANDLE hProcess = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE, pid);
            if (hProcess == NULL)
                continue; // May not have privileges

            TCHAR processPath[MAX_PATH];
            if (!GetModuleFileNameEx(hProcess, NULL, processPath, sizeof(processPath) / sizeof(TCHAR)))
            {
                CloseHandle(hProcess);
                continue; // Handle error
            }

            std::wstring processPathStr(processPath);

            if (processPathStr.find(UwpAppsTag) != std::wstring::npos ||
                processPathStr.find(WindowsPath) != std::wstring::npos ||
                processPathStr.find(WindowsPathUpperCase) != std::wstring::npos) {
                // processPath contains one of the strings

                CloseHandle(hProcess);
                continue;
            }

            // final process list

            std::wstring description = L"";

            DWORD handle;
            DWORD versionInfoSize = GetFileVersionInfoSize(processPath, &handle);
            if (versionInfoSize != 0)
            {
                std::vector<BYTE> versionInfo(versionInfoSize);
                if (GetFileVersionInfo(processPath, handle, versionInfoSize, versionInfo.data()))
                {
                    void* ptr;
                    UINT size;
                    if (VerQueryValue(versionInfo.data(), TEXT("\\StringFileInfo\\040904B0\\FileDescription"), &ptr, &size))
                    {
                        TCHAR* des = (TCHAR*)ptr;
                        description = std::wstring(des);
                    }
                }
            }

            ProcessDataModel model;
            model.pid = pid;
            model.title = title;
            model.describe = description;
            model.path = processPathStr;
            model.icon = GetIconBytes(model.path);

            processesExcept.push_back(model);

            CloseHandle(hProcess);
        }

        return processesExcept;
    }

    static std::vector<BYTE> GetIconBytes(const std::wstring& path) {
        Gdiplus::GdiplusStartupInput gdiplusStartupInput;
        ULONG_PTR gdiplusToken;
        Gdiplus::GdiplusStartup(&gdiplusToken, &gdiplusStartupInput, nullptr);

        HICON hIcon;
        UINT iconIndex = 0; // Use first icon
        ExtractIconEx(path.c_str(), iconIndex, &hIcon, nullptr, 1);

        if (hIcon == nullptr) {
            // Handle the error
            return std::vector<BYTE>();
        }

        std::vector<BYTE> bytes;

        {
            Gdiplus::Bitmap bitmap(hIcon);
            DestroyIcon(hIcon);

            IStream* stream = nullptr;
            CreateStreamOnHGlobal(NULL, TRUE, &stream);

            CLSID pngClsid;
            CLSIDFromString(L"{557cf406-1a04-11d3-9a73-0000f81ef32e}", &pngClsid);
            bitmap.Save(stream, &pngClsid, nullptr);

            HGLOBAL hMem = NULL;
            GetHGlobalFromStream(stream, &hMem);
            DWORD dwSize = GlobalSize(hMem);
            LPVOID pMem = GlobalLock(hMem);

            bytes = std::vector<BYTE>(dwSize);
            memcpy(bytes.data(), pMem, dwSize);

            GlobalUnlock(hMem);
            stream->Release();
        }

        Gdiplus::GdiplusShutdown(gdiplusToken);

        return bytes;
    }
};
