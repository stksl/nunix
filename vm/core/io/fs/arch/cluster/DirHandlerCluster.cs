using System.ComponentModel;
using System.Net.Http.Headers;
using Nunix.Core;

namespace Nunix.IO;
public sealed class DirHandlerCluster : Cluster 
{
    const int itemSize = VMConsts.HASH_FILENAME_LENGTH + 4;
    /// <summary>
    /// Entries in current directory handler cluster.
    /// </summary>
    public Dictionary<FileName, ClusterAddress> Entries;
    /// <summary>
    /// </summary>
    /// <param name="cluster"></param>
    /// <exception cref="Exception"></exception>
    #pragma warning disable CS8618 
    public DirHandlerCluster(Cluster cluster) 
        : base(cluster.Header, cluster.Address, cluster.Data, cluster.Size) 
    {
        if (Header.GetType() != typeof(StartDirHandlerHeader) ||
            Header.GetType() != typeof(ClusterHeader))
            throw new Exception("Cannot create a directory handler cluster from a file/directory cluster!");
        
        LoadEntries(true, out _);

        nextAddr = Header.Next;
    }
    #pragma warning restore
    private void LoadEntries(bool recreate, out int loaded) 
    {
        loaded = 0;

        if (recreate)
            Entries = new Dictionary<FileName, ClusterAddress>(Size / itemSize - 1);

        for(int i = 0; i < Data.Length / itemSize; i++) 
        {
            byte[] entryHash = new byte[VMConsts.HASH_FILENAME_LENGTH];
            Array.Copy(Data, itemSize * i, entryHash, 0, entryHash.Length);
            ClusterAddress addr = BitConverter.ToUInt32(Data, itemSize * i + VMConsts.HASH_FILENAME_LENGTH);
            if (addr == 0) return;
            Entries[new FileName(entryHash)] = addr;
            loaded++;
        }
    }
    /// <summary>
    /// Whether the current list of directory handlers clusters contain the filename.
    /// </summary>
    /// <param name="cl_handler"></param>
    /// <param name="filename"></param>
    /// <returns></returns>
    public async Task<ClusterAddress?> ContainsGloballyAsync(ClusterHandler cl_handler, FileName filename) 
    {
        int additional = 1;
        while (!Entries.ContainsKey(filename)) 
        {
            try {
                await LoadNextEntries(cl_handler, additional);
            } catch (NullReferenceException) {
                LoadEntries(true, out _);
                return null;
            }
            if (additional < 16)
                additional++;
        }

        ClusterAddress addr = Entries[filename];

        LoadEntries(true, out _);

        return addr;
    }
    private GlobalClusterAddress? nextAddr;
    /// <summary>
    /// Loads new entries by reading async from the next <paramref name="expectedHandlerClusters"/> clusters.
    /// </summary>
    /// <param name="cl_handler"></param>
    /// <param name="expectedHandlerClusters"></param>
    /// <returns>
    /// Amount of actual next handlers read.
    /// </returns>
    /// <exception cref="NullReferenceException"></exception>
    public async Task<int> LoadNextEntries(ClusterHandler cl_handler, int expectedHandlerClusters) 
    {
        if (nextAddr == null || nextAddr.Value == GlobalClusterAddress.Zero)
            throw new NullReferenceException("There are no next clusters left!");

        int c = 0;
        Entries = new Dictionary<FileName, ClusterAddress>();

        // todo, replace with ThroughClustersAsync
        for(int i = 0; i < expectedHandlerClusters; i++, c++) 
        {
            DirHandlerCluster nextHandler = 
                new DirHandlerCluster(await cl_handler.GetClusterAsync(nextAddr.Value));
            
            foreach(var nextEntry in nextHandler.Entries) 
            {
                Entries[nextEntry.Key] = nextEntry.Value;
            }

            nextAddr = nextHandler.Header.Next;

            if (nextAddr == GlobalClusterAddress.Zero) 
            {
                break;
            }
        }

        return c;
    }
    public override byte[] GetBytes()
    {
        if (Size / itemSize < Entries.Count) 
            throw new FormatException($"Can't fit {Entries.Count} entires in {Size / itemSize} max entries directory handler cluster.");

        byte[] raw = new byte[Size];
        Header.GetBytes().CopyTo(raw, 0);
        int i = 0;
        foreach(var entry in Entries) 
        {
            entry.Key.GetBytes().CopyTo(raw, i * itemSize);

            BitConverter.GetBytes(entry.Value.LocalAddr).CopyTo(raw, i * itemSize + VMConsts.HASH_FILENAME_LENGTH);
        }
        return raw;
    }
}