namespace Nunix.Core;
public static class VMConsts 
{
    public const int MIN_RAM = 0xff_ff;
    public const long MAX_RAM = 0xff_ff_ff_ff
    #if extended
    * 4 // 16 gb
    #endif
    ;
    // 1 gb
    public const uint MIN_STORAGE = uint.MaxValue / 4;

    #region file system
    /// <summary>
    /// CAT header size
    /// </summary>
    public const int FS_HEADER_SIZE = 12;
    /// <summary>
    /// Bitmap global offset in the file system (in clusters)
    /// </summary>
    public const int BITMAP_OFFSET = 1;
    public const int HASH_FILENAME_LENGTH = 24;
    #endregion
 
}