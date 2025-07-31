using System.IO;
using System.Windows;

namespace SimpleWpf.ViewModel
{
    /// <summary>
    /// View model for directory to support RecursiveNodeViewModel directory tree implementation
    /// </summary>
    public class PathViewModel : DispatcherViewModelBase
    {
        public static readonly DependencyProperty BaseDirectoryProperty =
            DependencyProperty.Register("BaseDirectory", typeof(string), typeof(PathViewModel));

        public static readonly DependencyProperty FullPathProperty =
            DependencyProperty.Register("FullPath", typeof(string), typeof(PathViewModel));

        public static readonly DependencyProperty ShortPathProperty =
            DependencyProperty.Register("ShortPath", typeof(string), typeof(PathViewModel));

        public static readonly DependencyProperty IsDirectoryProperty =
            DependencyProperty.Register("IsDirectory", typeof(bool), typeof(PathViewModel));

        public static readonly DependencyProperty RecursionDepthProperty =
            DependencyProperty.Register("RecursionDepth", typeof(int), typeof(PathViewModel));

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(PathViewModel));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(PathViewModel));

        public string BaseDirectory
        {
            get { return (string)GetValue(BaseDirectoryProperty); }
            set { SetValueOverride(BaseDirectoryProperty, value); }
        }
        public string FullPath
        {
            get { return (string)GetValue(FullPathProperty); }
            set { SetValueOverride(FullPathProperty, value); }
        }
        public string ShortPath
        {
            get { return (string)GetValue(ShortPathProperty); }
            set { SetValueOverride(ShortPathProperty, value); }
        }
        public bool IsDirectory
        {
            get { return (bool)GetValue(IsDirectoryProperty); }
            set { SetValueOverride(IsDirectoryProperty, value); }
        }
        public int RecursionDepth
        {
            get { return (int)GetValue(RecursionDepthProperty); }
            set { SetValueOverride(RecursionDepthProperty, value); }
        }
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValueOverride(IsExpandedProperty, value); }
        }
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValueOverride(IsSelectedProperty, value); }
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
            this.IsDirectory = Directory.Exists(path);

            this.BaseDirectory = baseDirectory;
            this.FullPath = path;

            if (this.IsDirectory)
                this.ShortPath = new DirectoryInfo(path).Name;

            else
                this.ShortPath = System.IO.Path.GetFileName(path);

            var baseDepth = GetDirectoryDepth(this.BaseDirectory);
            var pathDepth = GetDirectoryDepth(this.FullPath);

            this.RecursionDepth = pathDepth - baseDepth;
        }

        private int GetDirectoryDepth(string path)
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
