using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Win32.SafeHandles;

namespace SimpleWpf.NativeIO
{
    // https://learn.microsoft.com/en-us/dotnet/api/microsoft.win32.safehandles.safefilehandle?view=net-9.0
    //
    internal class UnmanagedFileLoader : IDisposable
    {
        public const uint FILE_ATTRIBUTE_NORMAL = 0x80000000;
        public const short INVALID_HANDLE_VALUE = -1;
        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;
        public const uint CREATE_NEW = 1;
        public const uint CREATE_ALWAYS = 2;
        public const uint OPEN_EXISTING = 3;

        /*
        hFile = CreateFile(argv[1],               // file to open
                           GENERIC_READ,          // open for reading
                           FILE_SHARE_READ,       // share for reading
                           NULL,                  // default security
                           OPEN_EXISTING,         // existing file only
                           FILE_ATTRIBUTE_NORMAL | FILE_FLAG_OVERLAPPED, // normal file
                           NULL);                 // no attr. template
        */

        // Use interop to call the CreateFile function.
        // For more information about CreateFile,
        // see the unmanaged MSDN reference library.
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess,
          uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition,
          uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        private SafeFileHandle handleValue = null;

        public UnmanagedFileLoader(string path)
            => Load(path);

        public void Load(string path)
        {
            if (path == null || path.Length == 0)
                throw new ArgumentNullException(nameof(path));

            // Try to open the file.
            handleValue = CreateFile(path, GENERIC_READ, 0, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

            // If the handle is invalid,
            // get the last Win32 error
            // and throw a Win32Exception.
            if (handleValue.IsInvalid)
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        }

        public bool IsOpen()
        {
            return this.Handle != null && !this.Handle.IsInvalid && !this.Handle.IsClosed;
        }

        public SafeFileHandle Handle
        {
            get
            {
                if (!handleValue.IsInvalid)
                    return handleValue;

                return null;
            }
        }

        public void Dispose()
        {
            if (handleValue != null)
            {
                handleValue.Close();
                handleValue.Dispose();
                handleValue = null;
            }
        }
    }
}
