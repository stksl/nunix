namespace Nunix.IO;
/// <summary>
/// File system's file HANDLE. Meaning all the metadata of the file
/// </summary>
public struct CatFileHandle 
{
    public readonly Cluster FirstCluster;
    public readonly FullPath Location;
    public FileName Filename => Location.Path.Last!.Value;
    public FileClusterHeader Header => (FileClusterHeader)FirstCluster.Header;
    
    public CatFileHandle(Cluster firstCl, FullPath loc)
    {
        FirstCluster = firstCl;
        Location = loc;
    }
}