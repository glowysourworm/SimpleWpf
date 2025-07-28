using System.IO;
using System.Xml.Linq;

using SimpleWpf.Native.WinAPI.Data;

namespace SimpleWpf.Native.IO
{
    public struct FastDirectoryResult
    {
        public string FileName;
        public string Path;
        public bool IsDirectory;
        public long Size;
        public FileAttributes Attributes;
        public DateTime CreationTimeUTC;
        public DateTime LastAccessTimeUTC;
        public DateTime LastWriteTimeUTC;

        /// <summary>
        /// Creates a directory-only result. These results may be filled in with another API call. Maybe just the
        /// .NET managed call.
        /// </summary>
        public FastDirectoryResult(string directory)
        {
            this.Path = directory;
            this.IsDirectory = true;
            this.Attributes = FileAttributes.Directory;
        }

        public FastDirectoryResult(string directory, WIN32_FIND_DATA findData)
        {
            this.Size = CombineHighLowInts(findData.nFileSizeHigh, findData.nFileSizeLow);
            this.FileName = findData.cFileName;
            this.IsDirectory = false;
            this.Path = System.IO.Path.Combine(directory, findData.cFileName);
            this.Attributes = (FileAttributes)findData.dwFileAttributes;

            this.CreationTimeUTC = ConvertDateTime((uint)findData.ftCreationTime.dwHighDateTime, (uint)findData.ftCreationTime.dwLowDateTime);
            this.LastAccessTimeUTC = ConvertDateTime((uint)findData.ftLastAccessTime.dwHighDateTime, (uint)findData.ftLastAccessTime.dwLowDateTime);
            this.LastWriteTimeUTC = ConvertDateTime((uint)findData.ftLastWriteTime.dwHighDateTime, (uint)findData.ftLastWriteTime.dwLowDateTime);
        }

        public FastDirectoryResult(string directory, System32FindData findData)
        {
            this.Size = CombineHighLowInts(findData.nFileSizeHigh, findData.nFileSizeLow);
            this.FileName = findData.cFileName;
            this.Path = System.IO.Path.Combine(directory, findData.cFileName);
            this.IsDirectory = false;
            this.Attributes = (FileAttributes)findData.dwFileAttributes;

            this.CreationTimeUTC = ConvertDateTime((uint)findData.ftCreationTime_dwHighDateTime, (uint)findData.ftCreationTime_dwLowDateTime);
            this.LastAccessTimeUTC = ConvertDateTime((uint)findData.ftLastAccessTime_dwHighDateTime, (uint)findData.ftLastAccessTime_dwLowDateTime);
            this.LastWriteTimeUTC = ConvertDateTime((uint)findData.ftLastWriteTime_dwHighDateTime, (uint)findData.ftLastWriteTime_dwLowDateTime);
        }

        /// <summary>
        /// Needs comments from Win32API (or try pinvoke.net)
        /// </summary>
        private long CombineHighLowInts(uint high, uint low)
        {
            return ((long)high << 0x20) | low;
        }

        /// <summary>
        /// Needs comments from Win32API (or try pinvoke.net)
        /// </summary>
        private DateTime ConvertDateTime(uint high, uint low)
        {
            var utc = CombineHighLowInts(high, low);

            return DateTime.FromFileTimeUtc(utc);
        }
    }
}
