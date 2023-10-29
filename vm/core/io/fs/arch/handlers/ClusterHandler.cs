using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using Nunix.Core;

namespace Nunix.IO;
/// <summary>
/// Provides some simple I/O operations related to the file system.
/// </summary>
public class ClusterHandler : IHandler, IDisposable
{
    private readonly Stream stream;
    internal readonly CATHeader header;
    /// <summary>
    /// Total max clusters allowed (size of the file system maximum)
    /// </summary>
    public readonly uint MaxClusters; 
    public ClusterHandler(Stream _stream, CATHeader _header, ulong b_maxLength)
    {
        stream = _stream;
        header = _header;

        MaxClusters = (uint)(b_maxLength / header.ClusterSize);
    }
    /// <summary>
    /// Gets current cluster position.
    /// </summary>
    public uint ClusterPosition => (uint)(stream.Position / header.ClusterSize);
    /// <summary>
    /// Amount of clusters currently available.
    /// </summary>
    public uint ClusterCount => (uint)(stream.Length / header.ClusterSize);
    /// <summary>
    /// Increases stream length by <paramref name="additionalClusters"/>
    /// </summary>
    /// <param name="additionalClusters"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void IncreaseLength(ushort additionalClusters) 
    {
        if (ClusterCount >= MaxClusters) 
        {
            throw new ArgumentOutOfRangeException(nameof(additionalClusters));
        }
        
        stream.SetLength(stream.Length + additionalClusters * header.ClusterSize);
    }
    public void ShrinkLength(ushort clustersRemoved) 
    {
        if (ClusterCount * header.ClusterSize <= VMConsts.MIN_STORAGE) 
        {
            throw new ArgumentOutOfRangeException(nameof(clustersRemoved));
        }

        stream.SetLength(stream.Length - clustersRemoved * header.ClusterSize);
    }

    /// <summary>
    /// Advances current position to a specific cluster address
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="origin"></param>
    /// <returns></returns>
    public long AdvanceCluster(GlobalClusterAddress addr, SeekOrigin origin)
    {
        return stream.Seek(addr.GlobalAddr * header.ClusterSize, origin);
    }

    /// <summary>
    /// Gets the cluster specified by the <paramref name="addr"/> fully, pass NULL to get the current cluster.
    /// </summary>
    /// <param name="addr"></param>
    /// <returns></returns>
    public async Task<Cluster> GetClusterAsync(GlobalClusterAddress globalAddr)
    {
        byte[] raw = new byte[header.ClusterSize];

        AdvanceCluster(globalAddr, SeekOrigin.Begin);
        await stream.ReadAsync(raw, 0, raw.Length);

        ClusterHeader h = ClusterHeader.Parse(raw, 0);

        byte[] data = new byte[raw.Length - h.Size];
        Array.Copy(raw, h.Size, data, 0, data.Length);

        return new Cluster(h, globalAddr, data, header.ClusterSize);
    }
    public async Task<CatOperationStatus> UpdateClusterAsync(Cluster cluster)
    {
        byte[] bytes = cluster.GetBytes();
        
        AdvanceCluster(cluster.Address, SeekOrigin.Begin);
        await stream.WriteAsync(bytes.ToArray(), 0, bytes.Length);
        return CatOperationStatus.Succeed;
    }

    /// <summary>
    /// links nexts and prevs in the order as passed.
    /// </summary>
    /// <param name="clusters"></param>
    /// <returns></returns>
    public async Task<CatOperationStatus> LinkClustersAsync(IList<ClusterAddress> clusters) 
    {
        for(int i = 0; i < clusters.Count; i++) 
        {
            Cluster cl = await GetClusterAsync(clusters[i].GetGlobalAddr(header));

            if (i > 0) cl.Header.Prev = clusters[i - 1].GetGlobalAddr(header);
            if (i + 1 < clusters.Count) cl.Header.Next = clusters[i + 1].GetGlobalAddr(header);

            var status = await UpdateClusterAsync(cl);

            if (status == CatOperationStatus.Failed) return CatOperationStatus.Failed;
        }
        
        return CatOperationStatus.Succeed;
    }
    /// <summary>
    /// Goes through all next (or prev) clusters applying <paramref name="predicate"/>, stops when true is returned
    /// </summary>
    /// <param name="startingCluster"></param>
    /// <param name="predicate"></param>
    /// <returns>
    /// Very last cluster or first cluster where <paramref name="predicate"/> returned true 
    /// </returns>
    public async Task<Cluster> ThroughClustersAsync(Cluster startingCluster, Func<Cluster, bool> predicate, bool forward) 
    {
        Cluster curr = startingCluster;

        GlobalClusterAddress nextAddr = curr.Header.Next;

        if (!forward) nextAddr = curr.Header.Prev;
        while (!predicate(curr) && nextAddr != GlobalClusterAddress.Zero) 
        {
            curr = await GetClusterAsync(curr.Header.Next);

            nextAddr = curr.Header.Next;
            if (!forward) nextAddr = curr.Header.Prev;
        }

        return curr;
    }
    public void Dispose()
    {
        Dispose(true);
    }
    private bool disposed = false;
    private void Dispose(bool disposing) 
    {
        if (disposing) 
        {
            if (!disposed) 
            {
                stream.Dispose();
                GC.SuppressFinalize(this);

                disposed = true;
            }
        }
    }
}