using Microsoft.Win32.SafeHandles;

using SimpleWpf.Native.WinAPI;

namespace SimpleWpf.Native.IO
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

        private SafeFileHandle handleValue = null;

        public UnmanagedFileLoader(string path)
            => Load(path);

        public void Load(string path)
        {
            if (path == null || path.Length == 0)
                throw new ArgumentNullException(nameof(path));

            // Try to open the file.
            handleValue = FileIO.CreateFile(path, GENERIC_READ, 0, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

            // If the handle is invalid,
            // get the last Win32 error
            // and throw a Win32Exception.
            if (handleValue.IsInvalid)
                FileIO.HandleLastWinAPIHRError();
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
