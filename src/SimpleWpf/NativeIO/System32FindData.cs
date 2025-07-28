using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SimpleWpf.NativeIO
{
    /// <summary>
    /// Contains information about the file that is found 
    /// by the FindFirstFile or FindNextFile functions.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    [BestFitMapping(false)]
    public class System32FindData
    {
        /// <summary> </summary>
        public FileAttributes dwFileAttributes;

        /// <summary> </summary>
        public uint ftCreationTime_dwLowDateTime;

        /// <summary> </summary>
        public uint ftCreationTime_dwHighDateTime;

        /// <summary> </summary>
        public uint ftLastAccessTime_dwLowDateTime;

        /// <summary> </summary>
        public uint ftLastAccessTime_dwHighDateTime;

        /// <summary> </summary>
        public uint ftLastWriteTime_dwLowDateTime;

        /// <summary> </summary>
        public uint ftLastWriteTime_dwHighDateTime;

        /// <summary> </summary>
        public uint nFileSizeHigh;

        /// <summary> </summary>
        public uint nFileSizeLow;

        /// <summary> </summary>
        public int dwReserved0;

        /// <summary> </summary>
        public int dwReserved1;

        /// <summary> </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string cFileName = string.Empty;

        /// <summary> </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
        public string cAlternateFileName = string.Empty;

        /// <summary>
        /// Returns a <see cref="string"/> that represents the current <see cref="object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents the current <see cref="object"/>.
        /// </returns>
        public override string ToString() => "File name=" + cFileName;
    }

    public enum WIN32_API_FILE_ERROR
    {
        NONE = 0,
        CANNOT_FIND_FILE = 2,
        NO_MORE_FILES = 18
    }

    public enum FINDEX_INFO_LEVELS
    {
        FindExInfoStandard = 0,
        FindExInfoBasic = 1
    }

    public enum FINDEX_SEARCH_OPS
    {
        FindExSearchNameMatch = 0,
        FindExSearchLimitToDirectories = 1,
        FindExSearchLimitToDevices = 2
    }

    // dwAdditionalFlags:
    public enum FIND_FIRST_EX_ADDITIONAL_FLAGS : int
    {
        FIND_FIRST_EX_NONE = 0,
        FIND_FIRST_EX_CASE_SENSITIVE = 1,
        FIND_FIRST_EX_LARGE_FETCH = 2
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    [BestFitMapping(false)]
    public struct WIN32_FIND_DATA
    {
        public uint dwFileAttributes;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
        public uint nFileSizeHigh;
        public uint nFileSizeLow;
        public uint dwReserved0;
        public uint dwReserved1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string cFileName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
        public string cAlternateFileName;

        public WIN32_FIND_DATA() { }
    }
}
