using System.IO;
using System.Security;
using System.Security.Permissions;

using SimpleWpf.Native.WinAPI;
using SimpleWpf.Native.WinAPI.Data;
using SimpleWpf.Native.WinAPI.Handle;

namespace SimpleWpf.Native.IO
{
    [SuppressUnmanagedCodeSecurity]
    public class FastDirectoryIO : IDisposable
    {
        private class DirectoryContext
        {
            public SafeFindHandle? Handle { get; set; }
            public string Path { get; private set; }

            public DirectoryContext(SafeFindHandle? handle, string path)
            {
                this.Handle = handle;
                this.Path = path;
            }
        }

        readonly string _baseDirectory;
        readonly string _filter;
        readonly SearchOption _searchOption;

        System32FindData _win32FindData;

        public FastDirectoryIO(string baseDirectory, string filter, SearchOption option)
        {
            _baseDirectory = baseDirectory;
            _filter = filter;
            _searchOption = option;
            _win32FindData = new System32FindData();
        }

        public IEnumerable<FastDirectoryResult> GetFiles()
        {
            // Procedure:
            //
            // 1) Get Complete Directory Listing
            // 2) Iterate Directories:  Get Files (flat-ly)
            //

            var result = new List<FastDirectoryResult>();

            // Flattened Directories
            //
            //var directories = GetFilesImpl(_baseDirectory, true);

            // .NET (NON-NATIVE) Directory Call!  
            //
            // Problem:  Couldn't get the WIN32API (Native) to return any folders for a directory
            //           that didn't have any files (just folder directory).
            //
            var directories = Directory.GetDirectories(_baseDirectory, "*", new EnumerationOptions()
            {
                RecurseSubdirectories = _searchOption == SearchOption.AllDirectories,
                MatchType = MatchType.Simple

            }).ToList();

            // Add Base Directory
            directories.Add(_baseDirectory);

            // Add Results During Iteration (performance)
            //
            foreach (var directory in directories.Select(x => new FastDirectoryResult(x)))
            {
                // Exclude the base directory from results
                if (directory.Path != _baseDirectory)
                {
                    result.Add(directory);
                }

                if (directory.Path == _baseDirectory ||
                    _searchOption == SearchOption.AllDirectories)
                {
                    // Get File Listing (flattened)
                    var directoryFiles = GetFromDirectory(directory.Path);

                    foreach (var file in directoryFiles)
                    {
                        // Win32 API may return directories during this operation, also...
                        if (!file.IsDirectory)
                        {
                            result.Add(file);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns all files in CURRENT directory regardless of type. Then, they may be iterated
        /// recursively to detail the folder tree.
        /// </summary>
        private IEnumerable<FastDirectoryResult> GetFromDirectory(string directory)
        {
            var result = new List<FastDirectoryResult>();
            var firstRead = true;
            DirectoryContext context = null;

            do
            {
                if (firstRead)
                {
                    // NATIVE CALL:  First read to directory
                    context = FirstNativeCall(directory);

                    // Create Result (with current Win32 Data)
                    if (context.Handle != null && !context.Handle.IsInvalid)
                    {
                        // File (we already have directories)
                        if (!_win32FindData.dwFileAttributes.HasFlag(FileAttributes.Directory))
                        {
                            result.Add(new FastDirectoryResult(directory, _win32FindData));
                        }
                    }

                    firstRead = false;
                }
                else
                {
                    // SEE WIN NATIVE API:  Continues the file (listing) for the directory
                    //
                    var nativeResult = NextNativeCall(context);

                    // Valid Result
                    if (nativeResult)
                    {
                        // File (we already have directories)
                        if (!_win32FindData.dwFileAttributes.HasFlag(FileAttributes.Directory))
                        {
                            result.Add(new FastDirectoryResult(directory, _win32FindData));
                        }
                    }

                    // Invalid Result:  Dispose -> Finish
                    else
                    {
                        context.Handle?.Dispose();
                        context.Handle = null;
                    }
                }

            } while (context.Handle != null && !context.Handle.IsInvalid);

            return result;
        }

        private bool NextNativeCall(DirectoryContext currentContext)
        {
            var result = FileIO.FindNextFile(currentContext.Handle, _win32FindData);

            // Error Check
            FileIO.HandleLastWinAPIError();

            return result;
        }

        private DirectoryContext FirstNativeCall(string currentDirectory)
        {
#pragma warning disable SYSLIB0003 // Type or member is obsolete
#pragma warning disable 618
            new FileIOPermission(FileIOPermissionAccess.PathDiscovery, currentDirectory).Demand();
#pragma warning restore SYSLIB0003 // Type or member is obsolete
#pragma warning restore 618

            // SEE WIN NATIVE API:  C:\(path)\(to)\(current)\(folder)\{filter = *.txt}
            //
            var searchPath = Path.Combine(currentDirectory, _filter);

            // Native Call: Directory + (some sort of wildcard search)
            var handle = FileIO.FindFirstFile(searchPath, _win32FindData);

            // Error Check
            FileIO.HandleLastWinAPIError();

            return new DirectoryContext(handle, currentDirectory);
        }

        public void Dispose()
        {

        }
    }
}
