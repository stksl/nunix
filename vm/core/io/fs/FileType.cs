namespace Nunix.IO;
[Flags]
public enum FileType : byte 
{
    File = 0x00,
    Directory = 0x04,
}