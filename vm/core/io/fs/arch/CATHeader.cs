
using System.Runtime.InteropServices;
using System.Xml.Schema;
namespace Nunix.IO;
internal interface ICATHeader
{
    // in bytes
    ushort ClusterSize {get; init;}
    ushort Version {get; init;}
    // where actual clusters begin from (everything before allocated for the file system)
    uint StartOffset {get; init;}
    // Currently not being used, some metadata can be stored
    int Reserved {get; init;}

}
public sealed class CATHeader : ICATHeader
{
    public ushort ClusterSize { get; init; }
    public ushort Version { get; init; }
    public uint StartOffset { get; init; }
    public int Reserved { get; init; }
    internal CATHeader() 
    {

    }
    internal CATHeader(ushort cSize, ushort ver, uint off, int resv)
    {
        ClusterSize = cSize;
        Version = ver;
        StartOffset = off;
        Reserved = resv; 
    }

    public static CATHeader FromBytes(byte[] bytes, int index) 
    {
        return new CATHeader(
        BitConverter.ToUInt16(bytes, index),
        BitConverter.ToUInt16(bytes, index + 2),
        BitConverter.ToUInt32(bytes, index + 4),
        BitConverter.ToInt32(bytes, index + 8)
        );
    }
}