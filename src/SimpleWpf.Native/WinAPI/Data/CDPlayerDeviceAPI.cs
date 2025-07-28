namespace SimpleWpf.Native.WinAPI.Data
{
    public static class CDPlayerDeviceAPI
    {
        public const uint IOCTL_CDROM_READ_TOC = 0x00024000;
        public const uint IOCTL_STORAGE_CHECK_VERIFY = 0x002D4800;
        public const uint IOCTL_CDROM_RAW_READ = 0x0002403E;
        public const uint IOCTL_STORAGE_LOAD_MEDIA = 0x002D480C;
        public const uint IOCTL_STORAGE_EJECT_MEDIA = 0x002D4808;

        public const uint GENERIC_READ = 0x80000000;
        public const uint FILE_SHARE_READ = 0x00000001;
        public const uint OPEN_EXISTING = 3;

        public static char[] DriveLetters()
        {
            return System.IO.DriveInfo
                            .GetDrives()
                            .Where(x => x.DriveType == System.IO.DriveType.CDRom)
                            .Select(x => x.Name.Replace(":", "").Replace("\\", "").Single())
                            .ToArray();
        }

        public static bool IsCDDrive(char driveLetter)
        {
            return DriveLetters().Contains(driveLetter);
        }
    }
}
