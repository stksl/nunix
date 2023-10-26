using NSubstitute;
using Nunix.Core;
using Nunix.IO;
using System.Reflection;
namespace Nunix.Test;

public class BitmapTests 
{
    internal static void reload_test_filesystem() 
    {
        var stream = File.Open(storagePath, FileMode.Create);
        byte[] header = new byte[VMConsts.FS_HEADER_SIZE];

        BitConverter.GetBytes((ushort)2048).CopyTo(header, 0);
        BitConverter.GetBytes((ushort)1).CopyTo(header, 2);
        BitConverter.GetBytes((uint)263303).CopyTo(header, 4);
        BitConverter.GetBytes((int)0).CopyTo(header, 8);

        stream.Write(header, 0, VMConsts.FS_HEADER_SIZE);

        stream.SetLength(VMConsts.MIN_STORAGE);
        stream.Close();
    }

    /// <summary>
    /// file system's path for tests
    /// </summary>
    internal const string storagePath = "/home/ubuntupc/nunix/vm/test/core/fs/storage.bin";
    [Fact]
    public async Task BitmapTest_Main() 
    {
        var stream = File.Open(storagePath, FileMode.Open);
        CATHandler handler = new CATHandler(stream);
        await handler.BootAsync(VMConsts.MIN_STORAGE);
        handler.Start();
        
        Bitmap? bitmap = 
            (Bitmap?)handler.GetType().
            GetField("bitmap", BindingFlags.Instance | BindingFlags.NonPublic)!.
            GetValue(handler);

        Assert.NotNull(bitmap);

        ClusterAddress[] addresses = 
        {
            0,
            134,
            255,
            256,
            667,
            1234,
        };

        foreach(var addr in addresses) 
        {
            try {
                await bitmap.ChangeClusterAsync(addr, true);
            } catch (Exception ex) {
                Assert.Fail(ex.Message);
            }
        }
        IList<ClusterAddress> ret_free_addrs = await bitmap.GetFreeClustersAsync(addresses.Length);

        Assert.Equal(addresses, ret_free_addrs);
        List<string> disposeErrors = new();
        foreach(var addr in addresses) 
        {
            try {
                await bitmap.ChangeClusterAsync(addr, false);
            } catch (Exception ex) {
                // thats pretty bad, the testable file system's file may be corrupted after that
                disposeErrors.Add(ex.Message);
            }
        }
        Assert.True(disposeErrors.Count == 0);

        MAIN.Shutdown();
    }
}