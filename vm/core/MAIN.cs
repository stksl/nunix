namespace Nunix.Core;
public static class MAIN 
{
    // for now
    public static void Shutdown() 
    {
        foreach(KeyValuePair<RegType, Register> reg in Registers.regsByTypes) 
        {
            reg.Value.value.Dispose();
        }
    }
}