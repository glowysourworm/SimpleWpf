using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;

using JetBrains.Annotations;

using Microsoft.Win32.SafeHandles;

namespace SimpleWpf.Native.WinAPI.Handle
{
    /// <summary>
    /// Wraps a FindFirstFile handle.
    /// </summary>
    [UsedImplicitly]
    internal sealed class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
#pragma warning disable SYSLIB0004 // Type or member is obsolete
#pragma warning disable SYSLIB0003
#pragma warning disable 618

        /// <summary> </summary>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport("kernel32.dll")]
        static extern bool FindClose(nint Handle);

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeFindHandle"/> class.
        /// </summary>
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        internal SafeFindHandle()
            : base(true) { }

#pragma warning restore 618
#pragma warning restore SYSLIB0003
#pragma warning restore SYSLIB0004 // Type or member is obsolete

        /// <summary>
        /// When overridden in a derived class, executes the code required to free the handle.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the handle is released successfully; otherwise, in the 
        /// event of a catastrophic failure, <see langword="false"/>. If so, it 
        /// generates a releaseHandleFailed MDA Managed Debugging Assistant.
        /// </returns>
        protected override bool ReleaseHandle() => FindClose(handle);
    }
}
