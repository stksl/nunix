
using System.Security.Cryptography;
using System.Text;

namespace Nunix.IO;
public class Cluster 
{
    public ClusterHeader Header {get; internal set;}
    public ClusterAddress Address {get; internal set;}
    /// <summary>
    /// Handle size = Cluster Size - Cluster Header Size (bytes)
    /// </summary>
    public IntPtr DataHandle {get; protected set;}
    public Cluster(ClusterHeader header, ClusterAddress addr, IntPtr dataHandle)
    {
        Header = header;
        Address = addr;
        DataHandle = dataHandle;
    }
}
public sealed class DirectoryHandlerCluster : Cluster 
{
    public Dictionary<FileNameChecksum, ClusterAddress> files;
    public DirectoryHandlerCluster(ClusterHeader header, ClusterAddress addr, IntPtr handle) 
        : base(header, addr, handle)
    {
        
    }
}
public sealed class FileNameChecksum
{
    private byte[] bytes;
    public FileNameChecksum(string filename, int size)
    {
        bytes = SHA256.HashData(Encoding.UTF8.GetBytes(filename));

        Array.Resize(ref bytes, size);
    }
    public override int GetHashCode()
    {
        // just first 4 bytes of the checksum
        return BitConverter.ToInt32(bytes, 0);
    }
    /// <summary>
    /// Both have to be equal size.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        if (obj is not FileNameChecksum other) return false;

        for(int i = 0; i < bytes.Length; i++) 
        {
            if (bytes[i] != other.bytes[i]) return false;
        }
        return true;
    }
    public static bool operator==(FileNameChecksum left, FileNameChecksum right) 
    {
        return left.Equals(right);
    }
    public static bool operator!=(FileNameChecksum left, FileNameChecksum right) 
    {
        return !left.Equals(right);
    }
}
