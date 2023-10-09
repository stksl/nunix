namespace Nunix.Core;
public static class VMConsts 
{
    public const int MIN_RAM = 0xff_ff;
    public const long MAX_RAM = 0xff_ff_ff_ff
    #if extended
    * 4 // 16 gb
    #endif
    ;

    public const uint MIN_STORAGE = 0xff_ff_ff_ff;
    // cat header size
    public const int FS_HEADER_SIZE = 8;
}