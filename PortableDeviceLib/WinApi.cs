using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace PortableDeviceLib
{
  public class WinApi
  {
    [Flags]
    public enum DesiredAccess : uint
    {
      GENERIC_READ = 0x80000000,
      GENERIC_WRITE = 0x40000000
    }
    [Flags]
    public enum ShareMode : uint
    {
      FILE_SHARE_NONE = 0x0,
      FILE_SHARE_READ = 0x1,
      FILE_SHARE_WRITE = 0x2,
      FILE_SHARE_DELETE = 0x4,

    }

    public enum CreationDisposition : uint
    {
      CREATE_NEW = 1,
      CREATE_ALWAYS = 2,
      OPEN_EXISTING = 3,
      OPEN_ALWAYS = 4,
      TRUNCATE_EXSTING = 5
    }

    [Flags]
    public enum FlagsAndAttributes : uint
    {
      FILE_ATTRIBUTES_ARCHIVE = 0x20,
      FILE_ATTRIBUTE_HIDDEN = 0x2,
      FILE_ATTRIBUTE_NORMAL = 0x80,
      FILE_ATTRIBUTE_OFFLINE = 0x1000,
      FILE_ATTRIBUTE_READONLY = 0x1,
      FILE_ATTRIBUTE_SYSTEM = 0x4,
      FILE_ATTRIBUTE_TEMPORARY = 0x100,
      FILE_FLAG_WRITE_THROUGH = 0x80000000,
      FILE_FLAG_OVERLAPPED = 0x40000000,
      FILE_FLAG_NO_BUFFERING = 0x20000000,
      FILE_FLAG_RANDOM_ACCESS = 0x10000000,
      FILE_FLAG_SEQUENTIAL_SCAN = 0x8000000,
      FILE_FLAG_DELETE_ON = 0x4000000,
      FILE_FLAG_POSIX_SEMANTICS = 0x1000000,
      FILE_FLAG_OPEN_REPARSE_POINT = 0x200000,
      FILE_FLAG_OPEN_NO_CALL = 0x100000
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr CreateFile(
        string lpFileName, uint dwDesiredAccess,
        uint dwShareMode, IntPtr lpSecurityAttributes,
        uint dwCreationDisposition,
        uint dwFlagsAndAttributes, IntPtr hTemplateFile);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern public bool WriteFile(IntPtr handle, IntPtr buffer,
     int numBytesToWrite, IntPtr numBytesWritten, [In] ref NativeOverlapped lpOverlapped);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool CloseHandle(IntPtr hObject);

  }
}
