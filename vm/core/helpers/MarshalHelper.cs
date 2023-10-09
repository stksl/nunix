using System.Runtime.InteropServices;

namespace Nunix.Core.Extensions;
internal static class MarshalHelper
{
    public static IntPtr AllocHGlobal(int nBytes, bool zeroFill)
    {
        IntPtr ptr = Marshal.AllocHGlobal(nBytes);
        if (!zeroFill) return ptr;

        for (int i = 0; i < nBytes; i++)
        {
            Marshal.WriteByte(ptr, i, 0);
        }
        return ptr;
    }
}