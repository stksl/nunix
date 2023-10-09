namespace Nunix.Core;
[Flags]
public enum AsmSize : int
{
    Byte = 1,
    Word = 1 << 1,
    Dword = 1 << 2,
    Qword = 1 << 3,
    Oword = 1 << 4,
    Yword = 1 << 5,
    Zword = 1 << 6,
}