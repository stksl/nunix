
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
    /// <summary>
    /// References all clusters (adds next and prev to the headers).
    /// </summary>
    /// <param name="addresses"></param>
    /// <returns></returns>
    public static void ReferenceClusters(params Cluster[] clusters) 
    {
        for(int i = 1; i < clusters.Length - 1; i++) 
        {
            clusters[i].Header.Prev = clusters[i - 1].Address;
            clusters[i].Header.Next = clusters[i + 1].Address;
        }
    }
    public static Task ReferenceClustersAsync(params Cluster[] clusters) 
        => Task.Run(() => ReferenceClusters(clusters)); 
    public virtual byte[] GetBytes()
    {
        byte[] raw = new byte[Size];
        Header.GetBytes().CopyTo(raw, 0);
        Data.CopyTo(raw, Header.Size);
        return raw;
    }
}
