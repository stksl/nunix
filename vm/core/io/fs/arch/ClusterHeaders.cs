using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Nunix.IO;

public class ClusterHeader
{
    public readonly ClusterType ClusterType;
    public readonly GlobalClusterAddress Next;
    public readonly GlobalClusterAddress Prev;

    public int Size => this is FileClusterHeader fh ? (int)ClusterType + (int)fh.FileType : (int)ClusterType;
    internal ClusterHeader(ClusterType type, GlobalClusterAddress next, GlobalClusterAddress prev)
    {
        ClusterType = type;
        Next = next;
        Prev = prev;
    }
    public static ClusterHeader Parse(byte[] bytes, int index) 
    {
        ClusterType type = bytes[index] == 0 ? ClusterType.Default : (ClusterType)bytes[index];

        GlobalClusterAddress next = new(BitConverter.ToUInt32(bytes, index + 1));
        GlobalClusterAddress prev = new(BitConverter.ToUInt32(bytes, index + 5));

        ClusterHeader h = new ClusterHeader(type, next, prev);

        // pipeline
        return type == ClusterType.StartingCluster ? FileClusterHeader.Parse(h, bytes, index + 9) : h;
    }
    public byte[] GetBytes() 
    {
        List<byte> bytes = new List<byte>()
        {
            (byte)ClusterType
        };
        // 4 bytes
        bytes.AddRange(BitConverter.GetBytes(Next.GlobalAddr));
        // 4 bytes
        bytes.AddRange(BitConverter.GetBytes(Prev.GlobalAddr));

        if (this is FileClusterHeader fh)
        {
            // 1 byte 
            bytes.Add((byte)fh.FileType);
            // 2 bytes
            bytes.AddRange(BitConverter.GetBytes(fh.Reserved));
            // 4 bytes
            bytes.AddRange(BitConverter.GetBytes(fh.ParentDir.LocalAddr));

            // inode => 16 bytes
            bytes.AddRange(BitConverter.GetBytes(fh.Inode.m_data[0]));
            bytes.AddRange(BitConverter.GetBytes(fh.Inode.m_data[1]));


        }
        if (this is DirClusterHeader dh)
        {
            // 4 bytes
            bytes.AddRange(BitConverter.GetBytes(dh.DirHandler.LocalAddr));
        }

        return bytes.ToArray();
    }
}
public class FileClusterHeader : ClusterHeader
{
    public readonly FileType FileType;
    public readonly ushort Reserved;
    public readonly ClusterAddress ParentDir;
    public readonly Inode Inode;
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
    public readonly ClusterAddress DirHandler;
    internal DirClusterHeader(FileClusterHeader fh,
        ClusterAddress dirHandler) : base(fh, fh.FileType, fh.Reserved, fh.ParentDir, fh.Inode)
    {
        DirHandler = dirHandler;
    }
    // pipelined
    internal static DirClusterHeader Parse(FileClusterHeader fh, byte[] bytes, int index) 
    {
        return new DirClusterHeader(fh, new(BitConverter.ToUInt32(bytes, index)));
    }
}
