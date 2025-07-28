namespace SimpleWpf.Native.WinAPI.Data.CDPlayerDevice
{
    /// <summary>
    /// Native enum appears to be one-to-one with the .NET enum System.IO.DriveType
    /// </summary>
    public enum DriveType : uint
    {
        DRIVE_UNKNOWN = 0,
        DRIVE_NO_ROOT_DIR,
        DRIVE_REMOVABLE,
        DRIVE_FIXED,
        DRIVE_REMOTE,
        DRIVE_CDROM,
        DRIVE_RAMDISK
    };

    public enum DeviceType : uint
    {
        DBT_DEVTYP_OEM = 0x00000000,
        DBT_DEVTYP_DEVNODE = 0x00000001,
        DBT_DEVTYP_VOLUME = 0x00000002,
        DBT_DEVTYP_PORT = 0x00000003,
        DBT_DEVTYP_NET = 0x00000004
    }

    public enum VolumeChangeFlags : ushort
    {
        DBTF_MEDIA = 0x0001,
        DBTF_NET = 0x0002
    }

    public enum TRACK_MODE_TYPE
    {
        YellowMode2,
        XAForm2,
        CDDA
    }
}
