namespace Nunix.IO;
/// <summary>
/// the values representing header size
/// </summary>
public enum ClusterType : byte 
{
    StartingCluster = 0x20,
    Default = 0x09,

    StartingHandlerCluster = StartingCluster + 3
}