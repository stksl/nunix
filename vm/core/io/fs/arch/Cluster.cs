
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Nunix.IO;
public class Cluster
{
    public readonly ClusterHeader Header;
    public readonly GlobalClusterAddress Address;
    public readonly byte[] Data;
    /// <summary>
    /// Full cluster size (including header)
    /// </summary>
    public readonly int Size;
    public Cluster(ClusterHeader header, GlobalClusterAddress addr, byte[] data, int size)
    {
        Header = header;
        Address = addr;

        Data = data;

        Size = size;
    }

    public unsafe byte[] GetBytes()
    {
        byte[] raw = new byte[Size];
        Header.GetBytes().CopyTo(raw, 0);
        Data.CopyTo(raw, Header.Size);
        return raw;
    }
}
