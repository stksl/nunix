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

    // for now
    public static void Start() 
    {
        ThreadPool.GetMinThreads(out int threads, out _);
        ThreadPool.SetMaxThreads(threads * 8, threads * 2);
    }
}