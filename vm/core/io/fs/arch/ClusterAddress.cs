using System.Diagnostics.CodeAnalysis;

namespace Nunix.IO;
public readonly struct ClusterAddress
{
    public readonly uint LocalAddr; 
    public ClusterAddress(uint localaddr)
    {
        LocalAddr = localaddr;
    }
    public GlobalClusterAddress GetGlobalAddr(CATHeader header) => new(LocalAddr + header.StartOffset);
    public override bool Equals([NotNullWhen(true)]object? obj)
    {
        return obj is ClusterAddress ca && LocalAddr == ca.LocalAddr;
    }
    public override int GetHashCode()
    {
        return (int)LocalAddr;
    }
    public static bool operator==(ClusterAddress left, ClusterAddress right) 
    {
        return left.Equals(right);
    }
    public static bool operator!=(ClusterAddress left, ClusterAddress right) 
    {
        return !left.Equals(right);
    }

    public static implicit operator ClusterAddress(uint localaddr) 
    {
        return new ClusterAddress(localaddr);
    }
}
/// <summary>
/// Used for clusters addresses, that are not local (bitmap clusters or file system metadata clusters)
/// </summary>
public readonly struct GlobalClusterAddress 
{
    public readonly uint GlobalAddr; 
    public GlobalClusterAddress(uint globaladdr)
    {
        GlobalAddr = globaladdr;
    }
    public override bool Equals([NotNullWhen(true)]object? obj)
    {
        return obj is GlobalClusterAddress ca && GlobalAddr == ca.GlobalAddr;
    }
    public override int GetHashCode()
    {
        return (int)GlobalAddr;
    }
    public static bool operator==(GlobalClusterAddress left, GlobalClusterAddress right) 
    {
        return left.Equals(right);
    }
    public static bool operator!=(GlobalClusterAddress left, GlobalClusterAddress right) 
    {
        return !left.Equals(right);
    }

}