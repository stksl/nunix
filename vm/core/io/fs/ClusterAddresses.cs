namespace Nunix.IO;

public static class ClusterAddresses 
{
    public static Task<Cluster> GetMainDirAsync(ClusterHandler cl_handler) 
        => cl_handler.GetClusterAsync(new ClusterAddress(0).GetGlobalAddr(cl_handler.header));

    
}
