using System.Runtime.InteropServices;

using SimpleWpf.Native.WinAPI.Data.CDPlayerDevice;

namespace SimpleWpf.Native.WinAPI
{
    public class DeviceIO
    {
        [DllImport("Kernel32.dll")]
        public extern static DriveType GetDriveType(string sDrive);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public extern static int CloseHandle(nint hObject);
        public const uint IOCTL_STORAGE_MEDIA_REMOVAL = 0x002D4804;

        [DllImport("Kernel32.dll", SetLastError = true)]
        public extern static int DeviceIoControl(nint hDevice,
                                                 uint IoControlCode,
                                                 nint lpInBuffer, uint InBufferSize,
                                                 nint lpOutBuffer, uint nOutBufferSize,
                                                 ref uint lpBytesReturned,
                                                 nint lpOverlapped);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public extern static int DeviceIoControl(nint hDevice, uint IoControlCode,
                                                 nint InBuffer, uint InBufferSize,
                                                 [Out] CDPlayerData OutTOC, uint OutBufferSize,
                                                 ref uint BytesReturned,
                                                 nint Overlapped);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public extern static int DeviceIoControl(nint hDevice, uint IoControlCode,
                                                 [In] PREVENT_MEDIA_REMOVAL InMediaRemoval, uint InBufferSize,
                                                 nint OutBuffer, uint OutBufferSize,
                                                 ref uint BytesReturned,
                                                 nint Overlapped);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public extern static int DeviceIoControl(nint hDevice,
                                                  uint IoControlCode,
                                                  [In] RAW_READ_INFO rri, uint InBufferSize,
                                                  [In, Out] byte[] OutBuffer, uint OutBufferSize,
                                                  ref uint BytesReturned,
                                                  nint Overlapped);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public extern static nint CreateFile(string FileName,
                                               uint DesiredAccess,
                                               uint ShareMode, nint lpSecurityAttributes,
                                               uint CreationDisposition, uint dwFlagsAndAttributes,
                                               nint hTemplateFile);
    }
}
