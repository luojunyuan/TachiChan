using System.Runtime.InteropServices;

namespace SplashScreenGdip;

internal partial class Gdiplus
{
    public const int GpStatus_Ok = 0;

    private const string GdiplusDll = "gdiplus.dll";

    // Startup, init 
#if !NET472
    [LibraryImport(GdiplusDll, SetLastError = true)]
    public static partial int GdiplusStartup(out nuint token, ref GdiplusStartupInput input, out GdiplusStartupOutput output);

    // Load

    [LibraryImport(GdiplusDll, SetLastError = true)]
    public static partial int GdipLoadImageFromFile([MarshalAs(UnmanagedType.LPWStr)] string filename, out nint image);

    [LibraryImport(GdiplusDll, SetLastError = true)]
    public static partial int GdipLoadImageFromStream(nint stream, out nint image);

    // Draw 

    [LibraryImport(GdiplusDll, SetLastError = true)]
    public static partial int GdipCreateFromHDC(nint hdc, out nint graphics);

    [LibraryImport(GdiplusDll, SetLastError = true)]
    public static partial int GdipDrawImageRectI(nint graphics, nint image, int x, int y, int width, int height);

    // Release

    [LibraryImport(GdiplusDll, SetLastError = true)]
    public static partial void GdiplusShutdown(nuint token);

#else
   [DllImport("gdiplus.dll", CharSet = CharSet.Unicode, EntryPoint = "GdiplusStartup", ExactSpelling = true, SetLastError = true)]
    internal static extern int GdiplusStartup([Out] out UIntPtr token, [In] ref GdiplusStartupInput input, [Out] out GdiplusStartupOutput output);

    [DllImport("gdiplus.dll", CharSet = CharSet.Unicode, EntryPoint = "GdipLoadImageFromFile", ExactSpelling = true, SetLastError = true)]
    public static extern int GdipLoadImageFromFile([In][MarshalAs(UnmanagedType.LPWStr)] string filename, [Out] out IntPtr image);

    [DllImport("gdiplus.dll", CharSet = CharSet.Unicode, EntryPoint = "GdipLoadImageFromStream", ExactSpelling = true, SetLastError = true)]
    public static extern int GdipLoadImageFromStream([In] IStream stream, [Out] out IntPtr image);

    [DllImport("gdiplus.dll", CharSet = CharSet.Unicode, EntryPoint = "GdipCreateFromHDC", ExactSpelling = true, SetLastError = true)]
    public static extern int GdipCreateFromHDC([In] IntPtr hdc, [Out] out IntPtr graphics);

    [DllImport("gdiplus.dll", CharSet = CharSet.Unicode, EntryPoint = "GdipDrawImageRectI", ExactSpelling = true, SetLastError = true)]
    public static extern int GdipDrawImageRectI([In] IntPtr graphics, [In] IntPtr image, [In] int x, [In] int y, [In] int width, [In] int height);

    [DllImport("gdiplus.dll", CharSet = CharSet.Unicode, EntryPoint = "GdiplusShutdown", ExactSpelling = true, SetLastError = true)]
    public static extern void GdiplusShutdown([In] UIntPtr token);
#endif

    [StructLayout(LayoutKind.Sequential)]
    internal struct GdiplusStartupOutput
    {
        public nint NotificationHook;
        public nint NotificationUnhook;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct GdiplusStartupInput
    {
        public uint GdiplusVersion;
        public nint DebugEventCallback;
        public int SuppressBackgroundThread; // bool
        public int SuppressExternalCodecs;   // bool
    }

    // BLENDFUNCTION in gdiplus, anther one in gdi32

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct BLENDFUNCTION
    {
        public static readonly byte AC_SRC_OVER = 0;
        public static readonly byte AC_SRC_ALPHA = 1;
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
    }
}

#if NET472

[ComImport(), Guid("0000000C-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IStream
{
int Read([In] IntPtr buf, [In] int len);
int Write([In] IntPtr buf, [In] int len);

[return: MarshalAs(UnmanagedType.I8)]
long Seek([In, MarshalAs(UnmanagedType.I8)] long dlibMove, [In] int dwOrigin);
void SetSize([In, MarshalAs(UnmanagedType.I8)] long libNewSize);

[return: MarshalAs(UnmanagedType.I8)]
long CopyTo(
        [In, MarshalAs(UnmanagedType.Interface)] IStream pstm,
        [In, MarshalAs(UnmanagedType.I8)] long cb,
        [Out, MarshalAs(UnmanagedType.LPArray)] long[] pcbRead);
void Commit([In] int grfCommitFlags);
void Revert();
void LockRegion(
       [In, MarshalAs(UnmanagedType.I8)] long libOffset,
       [In, MarshalAs(UnmanagedType.I8)] long cb,
       [In] int dwLockType);
void UnlockRegion(
       [In, MarshalAs(UnmanagedType.I8)] long libOffset,
       [In, MarshalAs(UnmanagedType.I8)] long cb,
       [In] int dwLockType);
void Stat([In] IntPtr pStatstg, [In] int grfStatFlag);

[return: MarshalAs(UnmanagedType.Interface)]
IStream Clone();
}


internal class GPStream : IStream
{
protected Stream dataStream;

// to support seeking ahead of the stream length...
long virtualPosition = -1;

public GPStream(Stream stream)
{
    if (!stream.CanSeek)
    {
        const int ReadBlock = 256;
        byte[] bytes = new byte[ReadBlock];
        int readLen;
        int current = 0;
        do
        {
            if (bytes.Length < current + ReadBlock)
            {
                byte[] newData = new byte[bytes.Length * 2];
                Array.Copy(bytes, newData, bytes.Length);
                bytes = newData;
            }
            readLen = stream.Read(bytes, current, ReadBlock);
            current += readLen;
        } while (readLen != 0);

        dataStream = new MemoryStream(bytes);
    }
    else
    {
        dataStream = stream;
    }
}

private void ActualizeVirtualPosition()
{
    if (virtualPosition == -1) return;

    if (virtualPosition > dataStream.Length)
        dataStream.SetLength(virtualPosition);

    dataStream.Position = virtualPosition;

    virtualPosition = -1;
}

public virtual IStream Clone()
{
    throw new NotImplementedException();
}

public virtual void Commit(int grfCommitFlags)
{
    dataStream.Flush();
    // Extend the length of the file if needed.
    ActualizeVirtualPosition();
}

public virtual long CopyTo(IStream pstm, long cb, long[] pcbRead)
{

    int bufsize = 4096; // one page
    IntPtr buffer = Marshal.AllocHGlobal(bufsize);
    if (buffer == IntPtr.Zero) throw new OutOfMemoryException();
    long written = 0;
    try
    {
        while (written < cb)
        {
            int toRead = bufsize;
            if (written + toRead > cb) toRead = (int)(cb - written);
            int read = Read(buffer, toRead);
            if (read == 0) break;
            if (pstm.Write(buffer, read) != read)
            {
                throw EFail("Wrote an incorrect number of bytes");
            }
            written += read;
        }
    }
    finally
    {
        Marshal.FreeHGlobal(buffer);
    }
    if (pcbRead != null && pcbRead.Length > 0)
    {
        pcbRead[0] = written;
    }

    return written;
}

public virtual Stream GetDataStream()
{
    return dataStream;
}

public virtual void LockRegion(long libOffset, long cb, int dwLockType)
{
}

protected static ExternalException EFail(string msg)
{
    throw new ExternalException(msg, unchecked((int)0x80004005)); // E_FAIL 
}

public virtual int Read(IntPtr buf, /* cpr: int offset,*/  int length)
{
    //        System.Text.Out.WriteLine("IStream::Read(" + length + ")");
    byte[] buffer = new byte[length];
    int count = Read(buffer, length);
    Marshal.Copy(buffer, 0, buf, length);
    return count;
}

public virtual int Read(byte[] buffer, /* cpr: int offset,*/  int length)
{
    ActualizeVirtualPosition();
    return dataStream.Read(buffer, 0, length);
}

public virtual void Revert()
{
    throw new NotImplementedException();
}

public virtual long Seek(long offset, int origin)
{
    // Console.WriteLine("IStream::Seek("+ offset + ", " + origin + ")");
    long pos = virtualPosition;
    if (virtualPosition == -1)
    {
        pos = dataStream.Position;
    }
    long len = dataStream.Length;
    switch (origin)
    {
        case StreamConsts.STREAM_SEEK_SET:
            if (offset <= len)
            {
                dataStream.Position = offset;
                virtualPosition = -1;
            }
            else
            {
                virtualPosition = offset;
            }
            break;
        case StreamConsts.STREAM_SEEK_END:
            if (offset <= 0)
            {
                dataStream.Position = len + offset;
                virtualPosition = -1;
            }
            else
            {
                virtualPosition = len + offset;
            }
            break;
        case StreamConsts.STREAM_SEEK_CUR:
            if (offset + pos <= len)
            {
                dataStream.Position = pos + offset;
                virtualPosition = -1;
            }
            else
            {
                virtualPosition = offset + pos;
            }
            break;
    }
    if (virtualPosition != -1)
    {
        return virtualPosition;
    }
    else
    {
        return dataStream.Position;
    }
}

public virtual void SetSize(long value)
{
    dataStream.SetLength(value);
}

public void Stat(IntPtr pstatstg, int grfStatFlag)
{
    STATSTG stats = new STATSTG
    {
        cbSize = dataStream.Length
    };
    Marshal.StructureToPtr(stats, pstatstg, true);
}

public virtual void UnlockRegion(long libOffset, long cb, int dwLockType)
{
}

public virtual int Write(IntPtr buf, /* cpr: int offset,*/ int length)
{
    byte[] buffer = new byte[length];
    Marshal.Copy(buf, buffer, 0, length);
    return Write(buffer, length);
}

public virtual int Write(byte[] buffer, /* cpr: int offset,*/ int length)
{
    ActualizeVirtualPosition();
    dataStream.Write(buffer, 0, length);
    return length;
}

[StructLayout(LayoutKind.Sequential)]
public class STATSTG
{
    public IntPtr pwcsName = IntPtr.Zero;
    public int type;
    [MarshalAs(UnmanagedType.I8)]
    public long cbSize;
    [MarshalAs(UnmanagedType.I8)]
    public long mtime;
    [MarshalAs(UnmanagedType.I8)]
    public long ctime;
    [MarshalAs(UnmanagedType.I8)]
    public long atime;
    [MarshalAs(UnmanagedType.I4)]
    public int grfMode;
    [MarshalAs(UnmanagedType.I4)]
    public int grfLocksSupported;

    public int clsid_data1;
    [MarshalAs(UnmanagedType.I2)]
    public short clsid_data2;
    [MarshalAs(UnmanagedType.I2)]
    public short clsid_data3;
    [MarshalAs(UnmanagedType.U1)]
    public byte clsid_b0;
    [MarshalAs(UnmanagedType.U1)]
    public byte clsid_b1;
    [MarshalAs(UnmanagedType.U1)]
    public byte clsid_b2;
    [MarshalAs(UnmanagedType.U1)]
    public byte clsid_b3;
    [MarshalAs(UnmanagedType.U1)]
    public byte clsid_b4;
    [MarshalAs(UnmanagedType.U1)]
    public byte clsid_b5;
    [MarshalAs(UnmanagedType.U1)]
    public byte clsid_b6;
    [MarshalAs(UnmanagedType.U1)]
    public byte clsid_b7;
    [MarshalAs(UnmanagedType.I4)]
    public int grfStateBits;
    [MarshalAs(UnmanagedType.I4)]
    public int reserved;
}

public class StreamConsts
{
    public const int LOCK_WRITE = 0x1;
    public const int LOCK_EXCLUSIVE = 0x2;
    public const int LOCK_ONLYONCE = 0x4;
    public const int STATFLAG_DEFAULT = 0x0;
    public const int STATFLAG_NONAME = 0x1;
    public const int STATFLAG_NOOPEN = 0x2;
    public const int STGC_DEFAULT = 0x0;
    public const int STGC_OVERWRITE = 0x1;
    public const int STGC_ONLYIFCURRENT = 0x2;
    public const int STGC_DANGEROUSLYCOMMITMERELYTODISKCACHE = 0x4;
    public const int STREAM_SEEK_SET = 0x0;
    public const int STREAM_SEEK_CUR = 0x1;
    public const int STREAM_SEEK_END = 0x2;
}
}
#endif