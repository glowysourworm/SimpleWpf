using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Win32.SafeHandles;

namespace SimpleWpf.NativeIO
{
    /*  https://stackoverflow.com/questions/3652386/how-to-use-a-safebuffer-in-c-sharp

        Q:  Where are any implementations of SafeBuffer? Can I use SafeMemoryMappedViewHandle?

        A:  You can't, it is an abstract class. The only visible concrete implementation of it is 
            SafeMemoryMappedViewHandle, a helper class for the classes in the System.IO.MemoryMappedFiles 
            namespace. It has a non-accessible constructor since it can only be properly initialized 
            by the plumbing that makes memory mapped files work.

            The use case is an IntPtr that maps to unmanaged memory, managed by a handle. Fairly rare 
            in the Windows API, MapViewOfFile or GlobalAllocPtr for example. If you want to create your 
            own then you have to derive from SafeBuffer so you can call its constructor and call, say, 
            AcquirePointer. Most of this is unsafe. What are you really trying to do?

    */

    internal class UnmanagedSafeBuffer : SafeBuffer
    {
        private SafeHandle _handle;

        public UnmanagedSafeBuffer(SafeHandle handle) : base(false)
        {
            _handle = handle;
        }

        protected override bool ReleaseHandle()
        {
            if (_handle != null)
            {
                _handle.Close();
                _handle.Dispose();
                _handle = null;
            }

            return true;
        }
    }
}
