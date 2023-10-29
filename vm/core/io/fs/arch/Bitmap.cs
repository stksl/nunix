using Nunix.Core;
using Nunix.Core.Extensions;

namespace Nunix.IO;

/// <summary>
/// Bitmap is basically a binary map of all a/unllocated clusters. 0 for unused, 1 for allocated
/// </summary>
public class Bitmap
{
    protected CATHandler handler { get; private set; }
    // clusters per bitmap cluster
    public int c_per_cl => (handler.header.ClusterSize - (int)ClusterType.Default) * 8;
    // bitmap length IN CLUSTERS (by default cluster size 2048b = 2^32 / 16384 = 262144 clusters)
    public uint Length => (uint)(uint.MaxValue + 1 / (long)c_per_cl);

    public Bitmap(CATHandler _handler)
    {
        handler = _handler;
    }
    
    public async Task<IList<ClusterAddress>> GetFreeClustersAsync(int expectedCount)
    {
        List<ClusterAddress> freeClusters = new List<ClusterAddress>(expectedCount);

        for (uint cl_addr = VMConsts.BITMAP_OFFSET; cl_addr <= Length; cl_addr++)
        {
            if (freeClusters.Count >= expectedCount) break;
            
            GlobalClusterAddress global_addr = new GlobalClusterAddress(cl_addr);
            Cluster bitmapCluster = await handler.cl_handler.GetClusterAsync(global_addr);

            IList<ClusterAddress> free_cls = await Task.Run(() => 
                readFreeClusters(bitmapCluster, cl_addr - VMConsts.BITMAP_OFFSET, expectedCount - freeClusters.Count)
            );
            freeClusters.AddRange(free_cls);
        }

        return freeClusters;
    }
    private IList<ClusterAddress> readFreeClusters(Cluster bitmapCluster, uint bitmapOffset, int maxCount)
    {
        List<ClusterAddress> addresses = new List<ClusterAddress>();
        for (int i = 0; i < bitmapCluster.Data.Length; i++)
        {
            if (addresses.Count >= maxCount) break;
            // managing 8 clusters 

            for (int mask = 0; mask < 8; mask++)
            {
                if (((bitmapCluster.Data[i] >> (7 - mask)) & 0b1) == 0)
                {
                    ClusterAddress addr =
                        (uint)(bitmapOffset * c_per_cl + i * 8 + mask);

                    addresses.Add(addr);
                }
            }
        }

        return addresses;
    }
    /// <summary>
    /// Allocates clusters' addresses in bitmap.
    /// </summary>
    /// <param name="addresses"></param>
    /// <returns></returns>
    /// <exception cref="SystemException"></exception>
    public async Task ChangeClusterAsync(ClusterAddress localaddr, bool allocate)
    {
        GlobalClusterAddress bitmapClusterOffset =
            new(localaddr.LocalAddr / (uint)c_per_cl + VMConsts.BITMAP_OFFSET);

        Cluster bitmapCluster = await handler.cl_handler.GetClusterAsync(bitmapClusterOffset);

        int index = (int)(localaddr.LocalAddr % c_per_cl) / 8;

        uint nBit = localaddr.LocalAddr % 8;
        int res = bitmapCluster.Data[index] | 0b1000_0000 >> (int)nBit;

        if (!allocate) res ^= 0b1000_0000 >> (int)nBit;

        bitmapCluster.Data[index] = (byte)res;
        
        CatOperationStatus status = await handler.cl_handler.UpdateClusterAsync(bitmapCluster);

        if (status != CatOperationStatus.Succeed) throw new SystemException("Unable to update a cluster!");
    }

    public async Task UnallocClusters(IList<ClusterAddress> addresses) 
    {
        foreach(var addr in addresses) 
        {
            await ChangeClusterAsync(addr, false);
        }
    }
}