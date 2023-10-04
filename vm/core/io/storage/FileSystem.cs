namespace Nunix.IO;
public interface IFileSystem 
{
    // in bytes
    ushort SectorSize {get; init;}
    // in sectors
    ushort ClusterSize {get; init;}

}
public class FileSystem : IFileSystem 
{
    public ushort SectorSize {get; init;}
    public ushort ClusterSize {get; init;}
    public FileSystem(ushort sectorSize, ushort clusterSize)
    {
        SectorSize = sectorSize;
        ClusterSize = clusterSize;
    }
}