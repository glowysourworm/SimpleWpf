using System.IO;

namespace SimpleWpf.ViewModel
{
    /// <summary>
    /// View model for directory to support RecursiveNodeViewModel directory tree implementation
    /// </summary>
    public class PathViewModel : RecursiveViewModelBase
    {
        private readonly string _baseDirectory;
        private readonly string _fullPath;
        private readonly string _shortPath;

        private bool _isDirectory;

        public bool IsDirectory
        {
            get { return _isDirectory; }
            set { this.RaiseAndSetIfChanged(ref _isDirectory, value); }
        }
        public string BaseDirectory
        {
            get { return _baseDirectory; }
        }
        public string FullPath
        {
            get { return _fullPath; }
        }
        public string ShortPath
        {
            get { return _shortPath; }
        }

        public PathViewModel(string baseDirectory, string path)
        {
            if (!Directory.Exists(baseDirectory))
                throw new ArgumentException("Directory does not exist! Must create PathViewModel with valid directory");

            if (!System.IO.Path.Exists(path))
                throw new ArgumentException("Path does not exist! Must create PathViewModel with valid path");

            if (string.IsNullOrEmpty(System.IO.Path.GetRelativePath(path, baseDirectory)))
                throw new ArgumentException("Path must be relative to base directory:  PathViewModel.cs");

            // Is Directory?
            _isDirectory = Directory.Exists(path);

            _baseDirectory = baseDirectory;
            _fullPath = path;

            if (_isDirectory)
                _shortPath = new DirectoryInfo(path).Name;

            else
                _shortPath = System.IO.Path.GetFileName(path);
        }

        public override string ToString()
        {
            return this.FullPath;
        }
    }
}
