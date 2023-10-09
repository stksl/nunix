namespace Nunix.IO;
public enum ClusterType : byte 
{
    Unused = 0x00,
    StartingCluster = 0x21,
    Allocated = 0x09,
}