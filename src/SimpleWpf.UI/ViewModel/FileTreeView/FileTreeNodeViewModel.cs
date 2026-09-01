using System.IO;

using SimpleWpf.UI.ViewModel.TreeView;

namespace SimpleWpf.UI.ViewModel.FileTreeView
{
    /// <summary>
    /// View model for using the SimpleTreeView with a file tree
    /// </summary>
    public class FileTreeNodeViewModel : TreeViewNodeModelBase
    {
        string _baseDirectory;
        string _fullPath;
        string _shortPath;
        DateTime _creationUtc;
        DateTime _lastAccessUtc;
        DateTime _lastWriteUtc;
        bool _isDirectory;
        int _directoryFileCount;

        public string BaseDirectory
        {
            get { return _baseDirectory; }
            set { this.RaiseAndSetIfChanged(ref _baseDirectory, value); }
        }
        public string FullPath
        {
            get { return _fullPath; }
            set { this.RaiseAndSetIfChanged(ref _fullPath, value); }
        }
        public string ShortPath
        {
            get { return _shortPath; }
            set { this.RaiseAndSetIfChanged(ref _shortPath, value); }
        }
        public DateTime CreationUtc
        {
            get { return _creationUtc; }
            set { this.RaiseAndSetIfChanged(ref _creationUtc, value); }
        }
        public DateTime LastAccessUtc
        {
            get { return _lastAccessUtc; }
            set { this.RaiseAndSetIfChanged(ref _lastAccessUtc, value); }
        }
        public DateTime LastWriteUtc
        {
            get { return _lastWriteUtc; }
            set { this.RaiseAndSetIfChanged(ref _lastWriteUtc, value); }
        }
        public bool IsDirectory
        {
            get { return _isDirectory; }
            set { this.RaiseAndSetIfChanged(ref _isDirectory, value); }
        }
        public int DirectoryFileCount
        {
            get { return _directoryFileCount; }
            set { this.RaiseAndSetIfChanged(ref _directoryFileCount, value); }
        }

        public FileTreeNodeViewModel(string baseDirectory, string path, int directoryFileCount)
            : base(GetDirectoryDepth(path) - GetDirectoryDepth(baseDirectory))
        {
            if (!Directory.Exists(baseDirectory))
                throw new ArgumentException("Directory does not exist! Must create PathViewModel with valid directory");

            if (!System.IO.Path.Exists(path))
                throw new ArgumentException("Path does not exist! Must create PathViewModel with valid path");

            if (string.IsNullOrEmpty(System.IO.Path.GetRelativePath(path, baseDirectory)))
                throw new ArgumentException("Path must be relative to base directory:  PathViewModel.cs");

            // Is Directory?
            this.IsDirectory = Directory.Exists(path);

            // This is sent in for performance purposes (also lazy loading)
            this.DirectoryFileCount = this.IsDirectory ? directoryFileCount : 0;
            this.IsLoaded = false;

            this.BaseDirectory = baseDirectory;
            this.FullPath = path;

            if (this.IsDirectory)
                this.ShortPath = new DirectoryInfo(path).Name;

            else
            {
                this.ShortPath = System.IO.Path.GetFileName(path);
                this.CreationUtc = System.IO.File.GetCreationTimeUtc(path);
                this.LastAccessUtc = System.IO.File.GetLastAccessTimeUtc(path);
                this.LastWriteUtc = System.IO.File.GetLastWriteTimeUtc(path);
            }
        }

        private static int GetDirectoryDepth(string path)
        {
            var directory = System.IO.Path.GetDirectoryName(path);

            return directory.Split("\\", StringSplitOptions.RemoveEmptyEntries).Length;
        }

        public override string ToString()
        {
            return this.FullPath;
        }
    }
}
