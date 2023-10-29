using System.Runtime.Intrinsics.X86;
using Nunix.IO;

public class DirClusterHandler : IHandler, IDisposable
{
    private readonly ClusterHandler handler;
    // for allocation purposes
    private readonly Bitmap bitmap;
    public DirClusterHandler(ClusterHandler _handler, Bitmap _bitmap)
    {
        handler = _handler;
        bitmap = _bitmap;
    }
    /// <summary>
    /// Allocates new clusters, adds as handlers and links at the end of the handlers list.
    /// </summary>
    /// <param name="expectedCount"></param>
    /// <param name="handlerCluster"></param>
    /// <returns>
    /// Amount of additional clusters linked.
    /// </returns>
    public async Task<int> AddHandlersAsync(int expectedCount, DirHandlerCluster handlerCluster) 
    {
        IList<ClusterAddress> freeAddresses = await bitmap.GetFreeClustersAsync(expectedCount);

        await handler.LinkClustersAsync(freeAddresses);
        // skipping to the very end
        Cluster last = await handler.ThroughClustersAsync(handlerCluster, _ => false, forward: true);

        last.Header.Next = freeAddresses[0].GetGlobalAddr(handler.header);
        await handler.UpdateClusterAsync(last);

        return freeAddresses.Count;
    }
    /// <summary>
    /// Removes <paramref name="expectedCount"/> clusters at the end of the handlers list.
    /// </summary>
    /// <param name="expectedCount"></param>
    /// <param name="handlerCluster"></param>
    /// <returns>
    /// Amount of handlers removed
    /// </returns>
    public async Task<int> RemoveHandlersAsync(int expectedCount, DirHandlerCluster handlerCluster) 
    {
        Cluster last = await handler.ThroughClustersAsync(handlerCluster, _ => false, forward: true);

        last = await handler.ThroughClustersAsync(handlerCluster, _ => expectedCount-- > 0, forward: false);

        // Todo, went back for expectedCount times, here we have to delete all the trailing clusters.
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
                handler.Dispose();
                GC.SuppressFinalize(this);

                disposed = true;
            }
        }
    }
}