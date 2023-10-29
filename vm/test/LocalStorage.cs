using Nunix.Core;
using Nunix.IO;

namespace Nunix.Test;
// just simple local storage
internal static class LocalStorage 
{
    public static Dictionary<object, object> Items = new();
    static LocalStorage() 
    {
        // fs 
        string[] paths = Environment.CurrentDirectory.Split('/');
        Items["ProjectPath"] = string.Join('/', paths.Take(paths.Length - 3)); // /obj/debug/net7.0
        Items["ProjectPath.Storage"] = (string)Items["ProjectPath"] + "/core/fs/storage.bin";
        reload_test_filesystem();

        CATHandler handler = new CATHandler(File.Open((string)Items["ProjectPath.Storage"], FileMode.Open));
        handler.BootAsync(VMConsts.MIN_STORAGE).Wait();
        handler.Start();
        Items["CatHandler"] = handler;
    }

    private static void reload_test_filesystem()
    {

        var stream = File.Open((string)Items["ProjectPath.Storage"], FileMode.Create);
        byte[] header = new byte[VMConsts.FS_HEADER_SIZE];

        BitConverter.GetBytes((ushort)2048).CopyTo(header, 0);
        BitConverter.GetBytes((ushort)1).CopyTo(header, 2);
        BitConverter.GetBytes((uint)263303).CopyTo(header, 4);
        BitConverter.GetBytes((int)0).CopyTo(header, 8);

        stream.Write(header, 0, VMConsts.FS_HEADER_SIZE);

        stream.SetLength(VMConsts.MIN_STORAGE);
        stream.Close();
    }
}