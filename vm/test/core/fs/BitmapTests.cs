using System.Reflection;
using Nunix.Core;
using Nunix.IO;

namespace Nunix.Test;

public class BitmapTests
{
    [Fact]
    public async Task Bitmap_AllocationTest()
    {
        CATHandler handler = (CATHandler)LocalStorage.Items["CatHandler"];

        Bitmap? bitmap =
            (Bitmap?)handler.GetType().
            GetProperty("bitmap", BindingFlags.Instance | BindingFlags.NonPublic)!.
            GetValue(handler);

        Assert.NotNull(bitmap);

        // addresses, that will be allocated
        ClusterAddress[] addresses ={ 0, 1, 5, 6 };

        foreach (var addr in addresses)
        {
            try
            {
                await bitmap.ChangeClusterAsync(addr, true);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        IList<ClusterAddress> ret_free_addrs = await bitmap.GetFreeClustersAsync(addresses.Length);
        ClusterAddress[] free_actual = {2, 3, 4, 7};
        Assert.Equal(ret_free_addrs, free_actual);

        foreach (var addr in addresses)
        {
            try
            {
                await bitmap.ChangeClusterAsync(addr, false);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

    }
}