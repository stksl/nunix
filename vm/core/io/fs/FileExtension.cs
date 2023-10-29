namespace Nunix.IO;
public readonly struct FileExtension 
{
    public readonly FileExtensionType Type;
    
}
public enum FileExtensionType 
{
    Unknown = 0,
    Executable
}