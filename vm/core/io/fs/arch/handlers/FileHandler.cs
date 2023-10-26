namespace Nunix.IO;

public sealed class FileHandler : IHandler
{
    private ClusterHandler cl_handler;
    public FileHandler(ClusterHandler clHandler)
    {
        cl_handler = clHandler;
    }

    public async Task<CatFile?> CreateFile(FullPath fullPath) 
    {
        return null!;
    }

}