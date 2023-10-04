using System.Runtime.InteropServices;

namespace Nunix.Core;
public unsafe class RegisterValue : IDisposable
{
    private object _sync = new object();
    private IntPtr valPtr;
    private readonly int offset;
    private readonly Register register;
    internal RegisterValue(Register reg)
    {
        offset = 8 - reg.Size;

        register = reg;
    }
    // inits the register value
    internal void Init() 
    {
        GPRegister? gp_reg = register as GPRegister;

        if (gp_reg == null || gp_reg.GetUpper() == null)
            valPtr = Marshal.AllocHGlobal(register.Size);
        
        else 
            valPtr = Registers.GetRegister(gp_reg.GetUpper()!.Value).Value.valPtr;
    }
    public void Dispose() 
    {
        Marshal.FreeHGlobal(valPtr);

        GC.SuppressFinalize(this);
    }
}