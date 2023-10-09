
using Nunix.Core;

namespace Nunix.IO;
public sealed class CATHandler
{
    private readonly FileStream stream;
    private CATHeader header;
    private bool running;
    /// <summary>
    /// Cluster size in bytes
    /// </summary>
    private int b_ClusterSize => header.SectorSize * header.ClusterSize;
#pragma warning disable 8618
    public CATHandler(FileStream _stream)
    {
        stream = _stream;

    }
#pragma warning restore
    /// <summary>
    /// Loads the header and other file system's metadata.
    /// </summary>
    /// <returns></returns>
    public async Task BootAsync()
    {
        byte[] headerBytes = new byte[VMConsts.FS_HEADER_SIZE];
        await stream.ReadAsync(headerBytes, 0, headerBytes.Length);

        header = CATHeader.FromBytes(headerBytes, 0);
    }
    /// <summary>
    /// Starts up the file system. It will run until the VM is closed.
    /// </summary>
    internal void Start()
    {
        if (running)
        {
            throw new Exception("CAT handler is already running.");
        }
        running = true;


    }

    public async Task<CatOperationStatus> CreateFile()
    {

    }
    private async Task<CatOperationStatus> createBitmap()
    {
    }

    public class Bitmap
    {
        private CATHandler handler;
        public Bitmap(CATHandler _handler)
        {
            handler = _handler;
        }

        public async Task<Cluster[]> GetFreeClusters(int count, IEnumerable<ClusterAddress> ignored)
        {
        }
        public Task<Cluster[]> GetFreeClusters(int count, params Cluster[] ignored) => GetFreeClusters(count, ignored.Select(i => i.Address));
    }
}
public class CATFile
{

}
public enum CatOperationStatus
{
    Failed,
    Succeed,
}