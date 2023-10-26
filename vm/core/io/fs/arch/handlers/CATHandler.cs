
using Nunix.Core;
namespace Nunix.IO;
public sealed class CATHandler : IDisposable
{
    private FileStream f_stream;
    internal ClusterHandler cl_handler {get; private set;}
    internal CATHeader header {get; private set;}
    internal Bitmap bitmap {get; private set;}
    private bool running;
#pragma warning disable 8618
    public CATHandler(FileStream _stream)
    {
        f_stream = _stream;
    }
#pragma warning restore
    /// <summary>
    /// Loads the header and other file system's metadata.
    /// </summary>
    /// <param name="max_storageSize"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task BootAsync(ulong max_storageSize)
    {
        if (running) throw new Exception("Handler has already started.");

        byte[] headerBytes = new byte[VMConsts.FS_HEADER_SIZE];
        await f_stream.ReadAsync(headerBytes, 0, headerBytes.Length);

        header = CATHeader.FromBytes(headerBytes, 0);

        cl_handler = new ClusterHandler(f_stream, header, max_storageSize);
        bitmap = new Bitmap(this);
    }
    /// <summary>
    /// Starts up the file system. It will run until the VM is closed.
    /// </summary>
    /// <exception cref="Exception"></exception>
    public void Start()
    {
        if (running)
        {
            throw new Exception("Handler has already started.");
        }

        running = true;
    }


    private bool disposed = false;
    public void Dispose() => Dispose(true);
    private void Dispose(bool disposing) 
    {
        if (disposing) 
        {
            if (!disposed) 
            {
                cl_handler.Dispose();
                GC.SuppressFinalize(this);

                disposed = true;
            }

        }
    }
}
public enum CatOperationStatus
{
    Failed,
    Succeed,
}