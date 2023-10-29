namespace Nunix.IO;

public sealed class FileHandler : IHandler
{
    private CATHandler catHandler;
    public FileHandler(CATHandler _catHandler)
    {
        catHandler = _catHandler;
    }
    /// <summary>
    /// Finds a file, and if so - returns the handle, otherwise null or exception 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="FileLoadException"></exception>
    /// <exception cref="FormatException"></exception>
    public async Task<CatFileHandle?> GetFileAt(FullPath path) 
    {
        LinkedListNode<FileName> dirNode = path.Path.First!;

        Cluster mainDir = await ClusterAddresses.GetMainDirAsync(catHandler.cl_handler);
        ClusterAddress handlerAddr = ((DirClusterHeader)mainDir.Header).DirHandler; 

        Cluster handlerCluster = 
            await catHandler.cl_handler.GetClusterAsync(handlerAddr.GetGlobalAddr(catHandler.header));
        DirHandlerCluster handler = new DirHandlerCluster(handlerCluster);

        while (dirNode != null) 
        {
            ClusterAddress? fileAddr = 
                await handler.ContainsGloballyAsync(catHandler.cl_handler, dirNode.Value);

            if (fileAddr == null) 
                throw new FileNotFoundException(nameof(path) + " part of the FULL PATH was not found. (no such file)");

            Cluster file = 
                await catHandler.cl_handler.GetClusterAsync(fileAddr.Value.GetGlobalAddr(catHandler.header));
            // endpoint of a list, found file 
            if (dirNode.Next == null) 
            {
                return file.Header is FileClusterHeader 
                ? new CatFileHandle(file, path) 
                : throw new FileLoadException("Internal error, directory entry references on non-file");
            }

            if (file.Header is not DirClusterHeader dirHeader)
                throw new FormatException($"Found file was not a directory! (hash: {dirNode.Value} )");
                
            Cluster nextHandler = 
                await catHandler.cl_handler.GetClusterAsync(dirHeader.DirHandler.GetGlobalAddr(catHandler.header));

            handler = new DirHandlerCluster(nextHandler);
            dirNode = dirNode.Next;
        }

        return null;
    }
    /// <summary>
    /// Allocates an item using <paramref name="cl_initialSize"/> clusters 
    /// </summary>
    /// <param name="fullPath"></param>
    /// <param name="cl_initialSize"></param>
    /// <returns></returns>
    public async Task<CatFileHandle?> CreateFileAsync(FullPath fullPath, FileType filetype, Inode inode, int cl_initialSize) 
    {
        Task freeClustersTask = catHandler.bitmap.GetFreeClustersAsync(cl_initialSize);

        LinkedListNode<FileName> dirNode = fullPath.Path.First!;

        CatFileHandle? filehandle = await GetFileAt(fullPath.RemoveLast());

        if (filehandle == null) 
        {
            return null;
        }
        if (filehandle.Value.Header is not DirClusterHeader dirHeader) 
            throw new FormatException("Cannot create file in non-directory file!");

        CatFileHandle handle = filehandle.Value;
        


        return null!;
    }

}