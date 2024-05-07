using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace TouchChan;

static class MessageBox
{
    public static void Show(string text, string title = "TachiChan", IntPtr parent = default) => _ = _MessageBox(parent, text, title, MB_TOPMOST | MB_SETFOREGROUND);

    [DllImport("user32")]
    private static extern int _MessageBox(IntPtr hWnd, string lpText, string lpCaption, int uType);

    // Not work if the process had win handle
    // https://stackoverflow.com/a/16105626
    private const int MB_TOPMOST = 0x00040000;
    private const int MB_SETFOREGROUND = 0x00010000;
}

static class Utils
{
    public static string StartTimeLeftBy() => $"({(DateTime.Now - Process.GetCurrentProcess().StartTime).Milliseconds})";

    public static Process GetProcessById(int pid) => Process.GetProcessById(pid);

    public static string ExtractLnkPath(string file)
    {
        try
        {
            var fileStream = File.Open(file, FileMode.Open, FileAccess.Read);
            using var fileReader = new BinaryReader(fileStream);
            fileStream.Seek(0x14, SeekOrigin.Begin);      // Seek to flags
            var flags = fileReader.ReadUInt32();          // Read flags
            if ((flags & 1) == 1)                         // Bit 1 set means we have to
            {
                // skip the shell item ID list
                fileStream.Seek(0x4c, SeekOrigin.Begin);  // Seek to the end of the header
                uint offset = fileReader.ReadUInt16();        // Read the length of the Shell item ID list
                fileStream.Seek(offset, SeekOrigin.Current);  // Seek past it (to the file locator info)
            }

            var fileInfoStartsAt = fileStream.Position;    // Store the offset where the file info
            // structure begins
            var totalStructLength = fileReader.ReadUInt32(); // read the length of the whole struct
            fileStream.Seek(0xc, SeekOrigin.Current);      // seek to offset to base pathname
            var fileOffset = fileReader.ReadUInt32();      // read offset to base pathname
            // the offset is from the beginning of the file info struct (fileInfoStartsAt)
            fileStream.Seek((fileInfoStartsAt + fileOffset), SeekOrigin.Begin); // Seek to beginning of
            // base pathname (target)
            var pathLength = (totalStructLength + fileInfoStartsAt) - fileStream.Position - 2; // read
            // the base pathname. I don't need the 2 terminating nulls.
            var linkTarget = fileReader.ReadBytes((int)pathLength); // should be unicode safe
            // error in VS but properly in context menu? "C:\Users\k1mlk\Desktop\金色ラブリッチェ.lnk"
#if !NET472
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
            var link = Encoding.GetEncoding(0).GetString(linkTarget);

            var begin = link.IndexOf("\0\0", StringComparison.Ordinal);
            if (begin <= -1)
                return link;

            var end = link.IndexOf(@"\\", begin + 2, StringComparison.Ordinal) + 2;
            end = link.IndexOf('\0', end) + 1;

            var firstPart = link.Substring(0, begin);
            var secondPart = link.Substring(end);

            return firstPart + secondPart;
        }
        catch (Exception exception)
        {
            return $"error when extract lnk. {exception}";
        }
    }

    public static PEType GetPEType(string path)
    {
        if (string.IsNullOrEmpty(path))
            return PEType.Unknown;

        try
        {
            using (var br = new BinaryReader(new FileStream(path,
                                                FileMode.Open,
                                                FileAccess.Read,
                                                FileShare.ReadWrite)))
            {
                if (br.BaseStream.Length < 0x3C + 4 || br.ReadUInt16() != 0x5A4D)
                    return PEType.Unknown;

                br.BaseStream.Seek(0x3C, SeekOrigin.Begin);
                var pos = br.ReadUInt32() + 4;

                if (pos + 2 > br.BaseStream.Length)
                    return PEType.Unknown;

                br.BaseStream.Seek(pos, SeekOrigin.Begin);
                var machine = br.ReadUInt16();

                return machine switch
                {
                    0x014C => PEType.X32,
                    0x8664 => PEType.X64,
                    0x01C4 => PEType.Arm,
                    0xAA64 => PEType.Arm64,
                    _ => PEType.Unknown,
                };
            }
        }
        catch
        {
            return PEType.Unknown;
        }
    }
}


enum PEType
{
    X32,
    X64,
    Arm,
    Arm64,
    Unknown
}
