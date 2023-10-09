using System.Runtime.InteropServices;
using System.Xml.Schema;

namespace Nunix.IO;
internal interface ICATHeader
{
    // in bytes
    ushort SectorSize {get; init;}
    // in sectors
    byte ClusterSize {get; init;}
    byte Version {get; init;}
    // where actual clusters begin from (everything before allocated for the file system)
    ushort StartOffset {get; init;}
    // Currently not being used, some metadata can be stored
    ushort Reserved {get; init;}

}

public sealed class CATHeader : ICATHeader
{
    public int b_ClusterSize => SectorSize * ClusterSize;

    public ushort SectorSize { get; init; }
    public byte ClusterSize { get; init; }
    public byte Version { get; init; }
    public ushort StartOffset { get; init; }
    public ushort Reserved { get; init; }
    internal CATHeader() 
    {

    }
    internal CATHeader(ushort secSize, byte cSize, byte ver, ushort off, ushort resv)
    {
        SectorSize = secSize;
        ClusterSize = cSize;
        Version = ver;
        StartOffset = off;
        Reserved = resv; 
    }

    public static CATHeader FromBytes(byte[] bytes, int index) 
    {
        ulong raw = BitConverter.ToUInt64(bytes, index);

        return new CATHeader(
        (ushort)(raw >> 48),
        (byte)(raw >> 40 & 0xff),
        (byte)(raw >> 32 & 0xff),
        (ushort)(raw >> 16 & 0xff_ff),
        (ushort)(raw & 0xff_ff)
        );
    }
}