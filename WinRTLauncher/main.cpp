#include "pch.h"

#include <winrt/Windows.ApplicationModel.h>
#include <winrt/Windows.ApplicationModel.AppService.h>
#include <winrt/Windows.Foundation.Collections.h>

#include<windows.h>

#include "FilterProcessService.hpp"

using namespace winrt;
using namespace Windows::Foundation;

using namespace winrt::Windows::Foundation;
using namespace winrt::Windows::ApplicationModel;
using namespace winrt::Windows::ApplicationModel::AppService;
using namespace winrt::Windows::Foundation::Collections;
using namespace std;

std::wstring getCommandLineArgumentValue(const std::wstring& commandLine, const std::wstring& argumentName) {
    size_t pos = commandLine.find(argumentName);
    if (pos != std::wstring::npos) {
        pos += argumentName.length();
        size_t endPos = commandLine.find(L' ', pos);
        if (endPos != std::wstring::npos) {
            return commandLine.substr(pos, endPos - pos);
        }
        return commandLine.substr(pos);
    }
    return L"";
}

//int main()
int __stdcall wWinMain(_In_ HINSTANCE, _In_opt_ HINSTANCE, _In_ PWSTR, _In_ int)
{
    int argc;
    auto argv = CommandLineToArgvW(GetCommandLineW(), &argc);
    std::wstring commandLine = GetCommandLineW();

    if (argc == 1) // this means no command arguments passed
    {
        MessageBox(nullptr, L"won't happen", L"WinRTLauncher", MB_OK);
        return 0;
    }

    // UWP-SelectProcess: Enumerate processes
    if (commandLine.find(L"--channel") != std::wstring::npos)
    {
        auto sss = FilterProcessService::Filter();

        init_apartment();
        //Uri uri(L"http://aka.ms/cppwinrt");
        //printf("1Hello, %ls!\n", uri.AbsoluteUri().c_str());

        AppServiceConnection connection;
        connection.AppServiceName(L"CommunicationService");
        connection.PackageFamilyName(Package::Current().Id().FamilyName());
        connection.OpenAsync().get();

        winrt::Windows::Foundation::Collections::ValueSet set;
        set.Insert(L"result", PropertyValue::CreateString(sss));
        connection.SendMessageAsync(set).get();
    }
    // UWP-SelectProcess: Attach to target process 
    else if (commandLine.find(L"--process-id") != std::wstring::npos)
    {
        wchar_t buffer[MAX_PATH];
        GetModuleFileName(nullptr, buffer, MAX_PATH);
        auto launcherPath = wstring(buffer);
        std::filesystem::path programPathRelative = L"..\\TouchChan\\TouchChan.exe";
        std::wstring programPath = std::filesystem::canonical(
            std::filesystem::path(launcherPath).parent_path() / programPathRelative);
        std::wstring programArgs = argv[4]; // winrt.exe TouchChan.exe pkgArg1 --process-id 1111

        programArgs = programPath + L" " + programArgs;

        STARTUPINFOW startupInfo = { sizeof(startupInfo) };
        PROCESS_INFORMATION processInfo;

        if (CreateProcessW(programPath.c_str(), const_cast<LPWSTR>(programArgs.c_str()), nullptr, nullptr, FALSE, 0, nullptr, nullptr, &startupInfo, &processInfo)) {

            CloseHandle(processInfo.hProcess);
            CloseHandle(processInfo.hThread);
        }
        else {
            DWORD errorCode = GetLastError();
            std::wstring errorMessage = L"プログラムの起動に失敗しました。エラーコード: " + std::to_wstring(errorCode);
            MessageBox(nullptr, errorMessage.c_str(), L"WinRTLauncher", MB_OK);
        }
    }
    else if (commandLine.find(L"--path") != std::wstring::npos)
    {
        wchar_t buffer[MAX_PATH];
        GetModuleFileName(nullptr, buffer, MAX_PATH);
        auto launcherPath = wstring(buffer);
        // 起動するプログラムのパスと引数を指定
        std::filesystem::path programPathRelative = L"..\\TouchChan\\TouchChan.exe";
        std::wstring programPath = std::filesystem::canonical(
            std::filesystem::path(launcherPath).parent_path() / programPathRelative);
        std::wstring programArgs = argv[4]; // winrt.exe TouchChan.exe pkgArg1 --path game.exe

        programArgs = programPath + L" \"" + programArgs + L"\"";

        // CreateProcessのパラメータを設定
        STARTUPINFOW startupInfo = { sizeof(startupInfo) };
        PROCESS_INFORMATION processInfo;

        //MessageBox(nullptr, programPath.c_str(), L"", MB_OK);
        //MessageBox(nullptr, programArgs.c_str(), L"", MB_OK);
        // プログラムの起動
        if (CreateProcessW(programPath.c_str(), const_cast<LPWSTR>(programArgs.c_str()), nullptr, nullptr, FALSE, 0, nullptr, nullptr, &startupInfo, &processInfo)) {
            // プログラムの起動に成功した場合
            //std::wcout << L"プログラムを起動しました。プロセスID: " << processInfo.dwProcessId << std::endl;

            // プロセスのハンドルを閉じる（不要になったら閉じること）
            CloseHandle(processInfo.hProcess);
            CloseHandle(processInfo.hThread);
        }
        else {
            // プログラムの起動に失敗した場合 
            DWORD errorCode = GetLastError();
            std::wstring errorMessage = L"プログラムの起動に失敗しました。エラーコード: " + std::to_wstring(errorCode);
            MessageBox(nullptr, errorMessage.c_str(), L"WinRTLauncher", MB_OK);
        }
    }

    return 0;
}

//std::wstring myWString =
//L"[{\n"
//L"\"ProcessId\": 12, \n"
//L"\"Title\": \"GameTile\", \n"
//L"\"Describe\": \"sth wrong\",\n"
//L"\"FullPath\": \"C:/asdasda/asda sd/asdasd.exe\",\n"
//L"\"IconBytes\": \"iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAAHYcAAB2HAY/l8WUAAA5TSURBVHhe3Zp7bJbVHcfJ4rKYbYYsS+Y2szXRJfuTZcmSbSoVjZaL8FIEBQXKnZZL37aAUC6tyFVs30KBloJtufaCrCqK0U0rCoKCvhZQUYFHMMucMWvcpoKX9+z3/Z3zO895nvfpBbBZ4STfnOdy3p7n8/39zuV53/a5kko8tisWH97g5cd2qvxhO+umx+oyzK2rv8RjO/vlx3ap2QQ/e9gO0nb10Lhn2k9s/jZumly9JR5ryCB4T4PvULMIftbQbWom6cSWFEm1JqvU1ZkNcUpzSn1Oe4m8ht+qZtxdr45vTqnjZAKpve1qzAaCTwq8H/mtpHqVd3edhicTkAnGCMqGL6+ObCD4BE94IQMQ+RkEnzfkMYL/1prAx6yrIBvi2Q0l8eG70uAReYHPHbJFHTPQUvuGcH1lZkPBiMZ8Wu4IPmiAjPu8IbUMP33wZh+aBBNcQ47V8HF726YrKBviseZ+BcMbVcCAGBlgxn7eEBP9wVvUtEGbLDRgA7UrvvbN5WdDXanXt2XJRyV7Ss7V7Vl8Lsdc/s4KljuC9+LGgMAQkOjfXcsGTB9c4xsAQFJbzTcBcJy3mXvGGMqGry4tG3Yv+ihzz5KzHkk9Ttqz+GxqD5lhbl920fBNXkF2o0IG6OjTxieQ/jr6SP1pg2rU1EHVFtQHjpYYpI8vMhso6vkAF8EANgH14rN1ptklF1rr+xaNaE4WZDcxvDsEAulPBuQaA6ZS9KcMrDLgIgO8yQcPyzGqPVn1daZ5hOiClP9LybnWAPziD60Bu3HMujwTCrOb6woBbwxImwCNAcgAmfy0ARsDcBevb5PmEdKLm/IBeGMA4MUAXV+aCYXZu0vIAAUD2AQ3+sOCBvDsT5OfNqDaGvAWZKKOOlKBNl/b2jxGsNiULzmnBXjHAIFP06IPk3Vxr6/5M10WH14boDOgKWSAv/lxZ/+plP6TszYYwK+1CMo3QV9zgFN8TEryPd3OPIoueHib8i64YwBAAyYs8gAuBqSaF3ndMqEo1hTz4bUADxOCQ8AYgK0vZYCdAMmASVnrLWiUNCjJmIDzZHXwvnmczlIetQNP18MmEDTLP/4w2TDf63CWxYxPwO1pBoQywBowDBlAKwAtgdoAPf4n3VVpIW1tAHVN0a7+is9d6bY6E/iBpg2qzp87coeaP2qHqptzLGgAapEDzaLIC7yrpoVUL/a8PQvP/Jo7cIqB98LwEBtAy6A/BPTrL157g0sgGZDlGwBYrkU419dSaffJEPdan3EDEiUP3LZGjbltlRp5y1I1OnOp2rngvWDK0zHXBhqp3pEBTQvPwIAUTCB5biZguSNIgseEFzbAzAEkawAtgTIJugbIBMgGAIhg3jS1wNlzuWauU+RTTtsUGVDe/sCAR9WYzNXqvv4r2IRJWeU6+o5s1I2C0C58WDDhXTYhHmtswSwfbYAMgfAuMDgJWgNoApxIBgBGZGH5WJ/zdQPvSj7TZ+yAcjV2QJm6n7JgdOYqde+ty9SIPy9R80ZuSwN2wcPRb4I46hZcNZq6ofiMt2B0SwIw+QQGSMB2ZkDaJOhmwECdARPvWmdB3qz6Sr1BkmO57sJGqc+42xNq7O1lCllw/22PkAkryYSHVTaZUDy6MWACywHHucBa+GKk/xnVWExCbY7r5rytikY0MBDACggynAlhA/Qc4GaAMwnSHMAGOLBvVF3QwI4ROBdjAmaYa2wAm0BZICZgKIwiE2J/KlbLJ+y18Ay+kJc6P/IELrWOuJYLDzUUn2YT5o3crWYPhQk6E2ACbYO55mXQNcDMAcFJUL8DYA9ABngCAniIjnmM63MfNCo7cM0aMO52PRS0CaspE/R8MOyP81VV/usW2IXWx8EM0NCnuQa01QJdb3/wPTX3niaOKjIBsHo4EHwoA3Df/R7AvgdgCGRt8HKyEhkC7kvg0q9ruecXXAPcTMCqIJPiQ6QS9VhRW5oBAkznftobeIl6WLsWnFLb5p00JujhAGDJgEKzDGIS9DNA3gO0AdMGbkrGM+t4s3V0owVKpYO6wBeUtBWDcB404A7UfibwynDrcnXPzaXqPloem2lGT4++iIaAGGBg+ZzucfRNBuyiGoIJi8c8aTMBc4IMCTcDXAMwBHIHb7bwKAJ51AK6BnQgaidmpGWAiE0wK8N9/bUJzcVePwJvF2gA2mNzrsF9YFoBHPBTfB3HO+efov3GKVU69mne6bkmhIeArAK5Q2pbXXgUgAAeMOFjt9byjenCgHIWrw7O8ogOG4pP5XC0DbCudbQFXo59cN8ALccEEptAkIh2HOCU/u4QYAOG1NYzcagY6BTXrPNcH+FaHwdE0HyPTTjfcQZoYTj4y6Pps0/DotOlDG4jH4R25cILcEB8/QO1atLfONUBDPB8vAmyAdvpRWhrJDyKgWQDNLQGs0ZsOO9cZ1FbudalAdgjlNvl0fTJhbIgwRkQAS3yoy2R1wKwC7+D9b5aSSZgtgc0R16b0SE8CkCsAGuARWJG8NqFlLTr3ACZFE0mmD5taVzotSIDAKszQca7n+Y+tJF7bIzQBnxAS+T7asXE53m8z6Z5YfbQraWmqw4LQwm4gXp9w5caNmSGFV2X7OgyAyA2gOYD06ct+MqMoD2JuG9AEB7HPriBftBEXmrKABiwbd57tPl6DjN+l/AoLrjAQdYEA8rnpoaQBai7ZQA0nrLB9GnL9KyqjLzBNR72CAIdzAA32gLvQJsa4NvpGJskLI/1c9/FrrFb3zi/7gALpD4P17if1pbeBiNgozT+joo0A2hvnsS+PHdIldo69x0n+hpYMoCjb67paAfhd7ABJIq+GFBb9LaqK+zaBIHUMmAbz6cQfTlncLpmoK1wvZsGYA4oDxhA63QCP1ZgczJ1UJV6cFRDIOLuMWAtPMCNdNRFOv23woA571AGnFC1hSdSjxUeS5guIwuDrzegXAPUGIJzc08MeU234WPoIgzw54B4dlMJNitYprB04atqvJ09NO4pP+oGPjDTG1gdedcEf/xvpejDgNqiE7T9Pq62FLapmniyw2+cBcrXFxrSnNvaALttujEEZAXQ3xegw6IRj+fLq6tvAr2o0B59ctZ69fD4vT48gUrk5ZyjzwaYtBfNIwPmniQDTlL0KQOK3qHoH1ebtQGqOv9opAkC4ypoSsf3UXdtgPNegB8u3fd33rfjtZXf2Or5RWXinRVq3YyDBlrDCjRMkJTX0Qc81QSPY2SAngBNBsCAgja1Kf9NVTX7iNow87VkIp4MbIVfI4jDlS6kHKMWSRtzz2/fWQb4kQc8NkIEHfoyU15ftQkzzLe2U7Iq+cEl+myAC+xEnSPPwvjHEDiZSjOAMkAbcFhV5h0kE1qtCYAAnG/CFzbCh+k4AG7ktu3QAP1Vmd4B3k+vx9iaBuGDJug3t+3mq+samhc2Ueq+FcgA3wQX3DGA0t/NgC1OBmycdUStJwPW5R1UFbn7rQk+4BeOEVRbeA0s57Y2ijBAR15Hn94B+q/kl5FoeF/4SVu/vW3jlWHaoGoyoZpn8zAwzgFswY3q9RxgM2BL4TEy4C1tAGXA+pmH1Nq8Ayoxfb8qm/qitzLn2YzDlZ8TCBQE8/W5OmTuHwreY8MCBsiEp/f+9BbYf5WaMrA6EjhdfiZgecSXF1MHbVTx7HoDCTia4R2ZyU7Xc952Zv5jqqYgSalPYz//KNI/VWngy6e/pB6d+oJaPfl5tWLCPu9AxWfq4Np/p0io1YG1nylck/oVvk+q0PdRv7r2P6xD6/4bbYDAT6alLRo2Wu7KgP08hgK+u583aheNZ0SzjSMKOBaN7RqClHpT/A0DfDS1YdZhjvi6mZTyea9o8GkafOWkZ9WyCU+rpeNb1IuP/pP1whqqSS84xyzcl7rsE6o/US+VfapeLv+XeiXxmWuAwD+ixhD8pLs2REJ2Jfyygy8zMGywP8DSOG7AGq98+v76ypmvqkpaIdbNOED1Aa5ZFNm1BFmR+7JK5O5n2DKCBfCaKX8l6OfUyon7CHqvWprzpCodu0cteaBZLRy9Sz297LTa6wjnVsuNzL29y86oZ5afUftWnFXPr/47GfMJGTAg0Y7IS9qPyVxFZpRFwnVPOgv0XLBFzbt3hfr4id976uXfqN6kf7SMV/sTR/DDSFmcx71d7tbwhBYN1x0ZA4Zux0/Z3sdP/CEZ9QC9QZ8+NULvbifeWVmK8T4lq/oy4SGbAR7eFqM67k1iA1DMLi/yV9uLExkQa0zih1D83ahOe5MYXor56bolHar7wj87CTxKVKe9SeYxg4WiWBoF15XC8ChRnUJ1C36GztOU2e9a5TVl2HalE37SrXata38Z2Q7KGXidbRcW3Y8uFzskCoY31YfhUaI6hVwDMn93LSvj+mv4POP676v2fTdyOzGg74++l9YO16Sda4C0EyVm/TTQtytq33Hp7pAAvPlIWonqFBIDACvXAAMoXK8wDy0GAKSzdmKA+/e6I/pM16WzIdEZPEpUp1CUAVDOwB/zdYDjPMoAKHbLDwPtetQAlOgh0dTlN7dRnUKRGfDMjXSu0xv3cS1sANoAVjJA2okBPFRofnCVrP2V7SMs+kz3i/5/3saKwuzH6wqzG7r1z9JRnUJiAB44J+s6lkDBFK9ZT3BiQJTGO5ObOwdk/PyagFrX3mDbhUXte7ZEdQqJAWEh0gIPiQEwBcexm3Xq97vpB4G/1+ND4FJLVKeQOwSwnEEyo7tKGwLOBFhv0h+6og2Iui8KG+Becz/rzgG476pl+S9su7DoMz1bojqFLseAzpbBKF3SRui7KlGdQhjnMKGz6ECYwaPata67IXAdpuA8SmjrftaVecyeK1Gd9iaZx+y5EtVpb5J5zJ4r1ElruNPeo5tazGP2XFGtv81AR9EP8P/UTa2qNSPjf+SUNZ5JvKiUAAAAAElFTkSuQmCC\"\n"
//L"}]\n";