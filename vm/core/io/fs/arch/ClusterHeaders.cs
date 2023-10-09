using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Nunix.IO;

public class ClusterHeader
{
    public ClusterType ClusterType { get; internal set; }
    public ClusterAddress Next { get; internal set; }
    public ClusterAddress Prev { get; internal set; }

    internal ClusterHeader(ClusterType type, ClusterAddress next, ClusterAddress prev)
    {
        ClusterType = type;
        Next = next;
        Prev = prev;
    }
    public int GetSize() 
    {
        return this is FileClusterHeader fh ? (int)ClusterType + (int)fh.FileType : (int)ClusterType;
    }
    public static ClusterHeader Parse(byte[] bytes, int index) 
    {
        ClusterType type = (ClusterType)bytes[index];

        ClusterAddress next = BitConverter.ToUInt32(bytes, index + 1);
        ClusterAddress prev = BitConverter.ToUInt32(bytes, index + 5);

        ClusterHeader h = new ClusterHeader(type, next, prev);

        // pipeline
        return type == ClusterType.StartingCluster ? FileClusterHeader.Parse(h, bytes, index + 9) : h;
    }
}
public class FileClusterHeader : ClusterHeader
{
    public FileType FileType { get; internal set; }
    public ushort Reserved { get; internal set; }
    public ClusterAddress ParentDir { get; internal set; }
    public Inode Inode { get; internal set; }
    internal FileClusterHeader(ClusterHeader header,
        FileType ftype, ushort resv, ClusterAddress parent, Inode inode) : base(header.ClusterType, header.Next, header.Prev)
    {
        FileType = ftype;
        Reserved = resv;
        ParentDir = parent;
        Inode = inode;
    }
    // pipelined
    internal static FileClusterHeader Parse(ClusterHeader header, byte[] bytes, int index) 
    {
        FileType ftype = (FileType)bytes[index];
        ushort resv = BitConverter.ToUInt16(bytes, index + 1);
        ClusterAddress parent = BitConverter.ToUInt32(bytes, index + 3);
        Inode inode = new Inode(bytes, index + 7);

        FileClusterHeader fh = new FileClusterHeader(header, ftype, resv, parent, inode);

        return ftype == FileType.Directory ? DirClusterHeader.Parse(fh, bytes, index + 23) : fh;
    }
}
public sealed class DirClusterHeader : FileClusterHeader
{
    public ClusterAddress DirHandler { get; internal set; }
    internal DirClusterHeader(FileClusterHeader fh,
        ClusterAddress dirHandler) : base(fh, fh.FileType, fh.Reserved, fh.ParentDir, fh.Inode)
    {
        DirHandler = dirHandler;
    }
    // pipelined
    internal static DirClusterHeader Parse(FileClusterHeader fh, byte[] bytes, int index) 
    {
        return new DirClusterHeader(fh, BitConverter.ToUInt32(bytes, index));
    }
}

public struct ClusterAddress : IComparable<ClusterAddress>
{
    public uint Addr; 
    public int CompareTo(ClusterAddress other)
    {
        return (int)(Addr - other.Addr);
    }
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is ClusterAddress ca && Addr == ca.Addr;
    }
    public override int GetHashCode()
    {
        return (int)Addr;
    }
    public static bool operator==(ClusterAddress left, ClusterAddress right) 
    {
        return left.Equals(right);
    }
    public static bool operator!=(ClusterAddress left, ClusterAddress right) 
    {
        return !left.Equals(right);
    }
    public static implicit operator ClusterAddress(uint u32) 
    {
        return new() {Addr = u32};
    }

}