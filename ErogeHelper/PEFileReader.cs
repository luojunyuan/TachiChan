// https://github.com/xupefei/Locale-Emulator/blob/master/LECommonLibrary/PEFileReader.cs
namespace ErogeHelper;

public enum PEType
{
    X32,
    X64,
    Arm,
    Arm64,
    Unknown
}

public static class PEFileReader
{
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
