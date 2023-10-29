using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Nunix.IO;

public class ClusterHeader
{
    public ClusterType ClusterType;
    public GlobalClusterAddress Next;
    public GlobalClusterAddress Prev;

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

        switch (type)
        {
            case ClusterType.StartingHandlerCluster:
                return StartDirHandlerHeader.Parse(h, bytes, index + 9);
            case ClusterType.StartingCluster:
                return FileClusterHeader.Parse(h, bytes, index + 9);
            default:
                return h;
        }
    }
    public virtual byte[] GetBytes()
    {
        List<byte> bytes = new List<byte>()
        {
            (byte)ClusterType
        };
        // 4 bytes
        bytes.AddRange(BitConverter.GetBytes(Next.GlobalAddr));
        // 4 bytes
        bytes.AddRange(BitConverter.GetBytes(Prev.GlobalAddr));

        return bytes.ToArray();
    }
}
public class FileClusterHeader : ClusterHeader
{
    public FileType FileType;
    public ushort Reserved;
    public ClusterAddress ParentDir;
    public Inode Inode;
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

    public override byte[] GetBytes()
    {
        List<byte> bytes = new List<byte>(base.GetBytes()) 
        {
            (byte)FileType
        };
        // 2 bytes
        bytes.AddRange(BitConverter.GetBytes(Reserved));
        // 4 bytes
        bytes.AddRange(BitConverter.GetBytes(ParentDir.LocalAddr));

        // inode => 16 bytes
        bytes.AddRange(BitConverter.GetBytes(Inode.m_data[0]));
        bytes.AddRange(BitConverter.GetBytes(Inode.m_data[1]));
        return bytes.ToArray();
    }
}
public sealed class DirClusterHeader : FileClusterHeader
{
    public ClusterAddress DirHandler;
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

    public override byte[] GetBytes()
    {
        List<byte> bytes = new List<byte>(base.GetBytes());
        bytes.AddRange(BitConverter.GetBytes(DirHandler.LocalAddr));

        return bytes.ToArray();
    }
}
public sealed class StartDirHandlerHeader : ClusterHeader
{
    public readonly uint FilesCount;
    public StartDirHandlerHeader(ClusterHeader header, uint filesCount)
        : base(header.ClusterType, header.Next, header.Prev)
    {
        FilesCount = filesCount;
    }
    internal static StartDirHandlerHeader Parse(ClusterHeader header, byte[] bytes, int index)
    {
        return new StartDirHandlerHeader(header, BitConverter.ToUInt32(bytes, index));
    }
    public override byte[] GetBytes()
    {
        List<byte> bytes = new List<byte>(base.GetBytes());
        bytes.AddRange(BitConverter.GetBytes(FilesCount));

        return bytes.ToArray();
    }
}