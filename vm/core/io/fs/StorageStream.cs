using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Nunix.IO;
/// <summary>
/// Provides some simple I/O operations related to the file system.
/// </summary>
public class StorageStream
{
    private readonly Stream stream;
    private readonly CATHeader header;
    public StorageStream(Stream _stream, CATHeader _header)
    {
        stream = _stream;
        header = _header;
    }
    /// <summary>
    /// Gets current cluster position.
    /// </summary>
    public uint ClusterPosition => (uint)(stream.Position / header.b_ClusterSize);
    /// <summary>
    /// Advances current position to a specific cluster address
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    public long AdvanceCluster(ClusterAddress addr, SeekOrigin origin)
    {
        return stream.Seek(addr.Addr * header.b_ClusterSize, origin);
    }
    /// <summary>
    /// Gets the cluster specified by the <paramref name="addr"/> fully, pass NULL to get the current cluster.
    /// </summary>
    /// <param name="addr"></param>
    /// <returns></returns>
    public async Task<Cluster> GetClusterAsync(ClusterAddress? addr)
    {
        byte[] raw = new byte[header.b_ClusterSize];
        if (addr != null)
            AdvanceCluster(addr.Value, SeekOrigin.Begin);
        await stream.ReadAsync(raw, 0, raw.Length);
        ClusterHeader h = ClusterHeader.Parse(raw, 0);
        IntPtr handle = IntPtr.Zero;
        unsafe
        {
            fixed (byte* p = raw)
            {
                handle = (IntPtr)p + h.GetSize();
            }
            return new Cluster(ClusterHeader.Parse(raw, 0), ClusterPosition, handle);
        }
    }
    public async Task<CatOperationStatus> UpdateClusterAsync(Cluster cluster)
    {
        ClusterHeader c_header = cluster.Header;
        List<byte> bytes = new List<byte>(header.b_ClusterSize)
        {
            (byte)c_header.ClusterType
        };
        // 4 bytes
        bytes.AddRange(BitConverter.GetBytes(c_header.Next.Addr));
        // 4 bytes
        bytes.AddRange(BitConverter.GetBytes(c_header.Prev.Addr));

        if (c_header is FileClusterHeader fh)
        {
            // 1 byte 
            bytes.Add((byte)fh.FileType);
            // 2 bytes
            bytes.AddRange(BitConverter.GetBytes(fh.Reserved));
            // 4 bytes
            bytes.AddRange(BitConverter.GetBytes(fh.ParentDir.Addr));

            // inode => 16 bytes
            bytes.AddRange(BitConverter.GetBytes(fh.Inode.m_data[0]));
            bytes.AddRange(BitConverter.GetBytes(fh.Inode.m_data[1]));


        }
        if (c_header is DirClusterHeader dh)
        {
            // 4 bytes
            bytes.AddRange(BitConverter.GetBytes(dh.DirHandler.Addr));
        }

        unsafe
        {
            byte* ptr = (byte*)cluster.DataHandle;

            for (int i = header.b_ClusterSize - bytes.Count; i >= 0; i--, ptr++)
            {
                bytes.Add(*ptr);
            }
        }
        
        AdvanceCluster(cluster.Address, SeekOrigin.Begin);
        await stream.WriteAsync(bytes.ToArray(), 0, bytes.Count);

        return CatOperationStatus.Succeed;
    }
}