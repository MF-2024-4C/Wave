using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quantum.FF.Utilities;

public static class UnsafeUtilities
{
    public static unsafe int AlignOf<T>() where T : unmanaged
    {
        return sizeof(AlignOfHelper<T>) - sizeof(T);
    }
    
    [StructLayout(LayoutKind.Sequential)]
    private struct AlignOfHelper<T> where T : unmanaged
    {
        public byte dummy;
        public T data;
    }
}