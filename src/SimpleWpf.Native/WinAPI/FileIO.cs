using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.Win32.SafeHandles;

using SimpleWpf.Native.WinAPI.Data;
using SimpleWpf.Native.WinAPI.Handle;

namespace SimpleWpf.Native.WinAPI
{
    internal static class FileIO
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern SafeFindHandle FindFirstFile(string filePath, [In][Out] System32FindData lpFindFileData);

        //[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        //static extern SafeFindHandle FindFirstFileEx([In] string lpFileName,
        //                                             [In] FINDEX_INFO_LEVELS fInfoLevelId,
        //                                             [In][Out] System32FindData lpFindFileData,
        //                                             [In] FINDEX_SEARCH_OPS fSearchOp,
        //                                             IntPtr lpSearchFilter,
        //                                             [In] FIND_FIRST_EX_ADDITIONAL_FLAGS dwAdditionalFlags);

        /// <summary>
        /// Native call for finding "next file" given the "first file". The handle is the previous file handle.
        /// </summary>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool FindNextFile(SafeFindHandle handleFindFile, [In][Out][MarshalAs(UnmanagedType.LPStruct)] System32FindData LpFindFileData);

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
        internal static extern SafeFileHandle CreateFile(string lpFileName, 
                                                         uint dwDesiredAccess,
                                                         uint dwShareMode, 
                                                         IntPtr lpSecurityAttributes, 
                                                         uint dwCreationDisposition,
                                                         uint dwFlagsAndAttributes, 
                                                         IntPtr hTemplateFile);

        /* NOT CURRENTLY USED */

        const uint File_Name_Normalized = 0x0;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [SuppressMessage("Globalization", "CA2101:Specify marshalling for P/Invoke string arguments")]
        static extern SafeFileHandle? CreateFile([MarshalAs(UnmanagedType.LPTStr)] string Filename,
                                                 [MarshalAs(UnmanagedType.U4)] FileAccess Access,
                                                 [MarshalAs(UnmanagedType.U4)] FileShare Share,
                                                 IntPtr SecurityAttributes, // optional SECURITY_ATTRIBUTES struct or IntPtr.Zero
                                                 [MarshalAs(UnmanagedType.U4)] FileMode CreationDisposition,
                                                 [MarshalAs(UnmanagedType.U4)] FileAttributes FlagsAndAttributes,
                                                 IntPtr TemplateFile);

        /// <summary>
        /// <see href="http://www.pinvoke.net/default.aspx/shell32/GetFinalPathNameByHandle.html"/>
        /// </summary>
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [SuppressMessage("Globalization", "CA2101:Specify marshalling for P/Invoke string arguments")]
        [SuppressMessage("Performance", "CA1838:Avoid 'StringBuilder' for P/Invokes")]
        static extern uint GetFinalPathNameByHandle(SafeFileHandle HFile, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder LpszFilePath, uint CchFilePath, uint DWFlags);

        /* ************************************************** */

        internal static void HandleLastWinAPIHRError()
        {
            var error = (WIN32_API_HR_FILE_ERROR)Marshal.GetHRForLastWin32Error();
            
            switch (error)
            {
                case WIN32_API_HR_FILE_ERROR.NONE:
                    break;
                default:
                    Marshal.ThrowExceptionForHR((int)error);
                    break;
            }
        }

        internal static void HandleLastWinAPIError()
        {
            var error = (WIN32_API_FILE_ERROR)Marshal.GetLastWin32Error();
            var errorMessage = Marshal.GetLastPInvokeErrorMessage();

            switch (error)
            {
                case WIN32_API_FILE_ERROR.NONE:
                    break;
                case WIN32_API_FILE_ERROR.CANNOT_FIND_FILE:
                    break;
                case WIN32_API_FILE_ERROR.NO_MORE_FILES:
                    //throw new IOException("Error in FastGetFiles Native Call:  " + errorMessage);
                    break;
                default:
                    throw new ApplicationException("Unhandled FastGetFiles Native Error Code:  " + errorMessage);
            }
        }
    }
}
