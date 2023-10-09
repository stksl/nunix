using System.Numerics;
using System.Runtime.InteropServices;
using Nunix.Core.Extensions;

namespace Nunix.Core;
public sealed unsafe class RegisterValue : IDisposable
{
    private IntPtr valPtr;
    private readonly int offset;
    private readonly Register register;
    public RawValueHandler Handler {get; private set;}
    // if this is lower general-purpose register = false, otherwise = true
    private bool disposable;
#pragma warning disable 8618
    internal RegisterValue(Register reg)
    {
        if (reg.IsGeneralPurpose)
            offset = (int)AsmSize.Qword - reg.Size;
        else offset = 0;

        register = reg;
    }
#pragma warning restore
    // inits the register value
    internal void Init() 
    {
        GPRegister? gp_reg = register as GPRegister;

        if (gp_reg == null || gp_reg.GetUpper() == null) 
        {
            // extension, zero fill
            valPtr = MarshalHelper.AllocHGlobal(register.Size, true);

            disposable = true;
        }
        else 
            valPtr = Registers.GetRegister(gp_reg.GetUpper()!.Value).value.valPtr;

        Handler = new RawValueHandler(valPtr, register.Size, offset);
    }
    public void Dispose() 
    {
        if (disposable) 
        {
            Marshal.FreeHGlobal(valPtr);
        }
        GC.SuppressFinalize(this);
    }
}
public sealed unsafe class RawValueHandler
{
    private object _sync = new object();
    // pointer to a raw value
    private IntPtr handle;
    // size of bytes to be handled
    private readonly int size;
    // global offset of handle (RAX and EAX point to the same data, but EAX globalOffset=4)
    private readonly int globalOffset;
    public RawValueHandler(IntPtr _handle, int _size, int _globalOffset)
    {
        handle = _handle;
        size = _size;
        globalOffset = _globalOffset;
    }
    /// <summary>
    /// Gets the handle with big-endian offset globally
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    public byte* GetHandle(int offset) 
    {
        return (byte*)(handle + globalOffset + size - offset - 1);
    }
    /// <summary>
    /// Loads the <paramref name="raw_ptr"/> value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="raw_ptr"></param>
    /// <param name="count"></param>
    public void LoadRawValue<T>(T* raw_ptr, int count) where T : unmanaged 
    {
        lock (_sync) 
        {
            byte* ptr = (byte*)raw_ptr;
            for(int offset = 0; offset < count; offset++) 
            {
                *GetHandle(offset) = *(ptr + offset);
            }
        }
    }
}