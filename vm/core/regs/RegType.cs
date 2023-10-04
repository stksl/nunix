namespace Nunix.Core;
public enum RegType : long 
{
    // Unkown register (error)
    Unkown = -1,

    #region General_Purpose
    RAX = 0x10,
    RBX,
    RCX,
    RDX,
    RDI,
    RSI,
    RSP,
    RBP,
    R8,R9,R10,R11,R12,R13,R14,R15,
    // 32 lower bits
    EAX = 0x20,
    EBX,
    ECX,
    EDX,
    EDI,
    ESI,
    ESP,
    EBP,
    R8d,R9d,R10d,R11d,R12d,R13d,R14d,R15d,
    // 16 lower bits
    AX = 0x30,
    BX,
    CX,
    DX,
    DI,
    SI,
    SP,
    BP,
    R8w,R9w,R10w,R11w,R12w,R13w,R14w,R15w,

    // 8 lower bits
    AL = 0x40,
    BL,
    CL,
    DL,
    DIL,
    SIL,
    SPL,
    BPL,
    R8b,R9b,R10b,R11b,R12b,R13b,R14b,R15b,
    #endregion
    #region SSE

    XMM0 = 0x80,
    XMM1,
    XMM2,
    XMM3,
    XMM4,
    XMM5,
    XMM6,
    XMM7,
    XMM8,
    XMM9,
    XMM10,
    XMM11,
    XMM12,
    XMM13,
    XMM14,
    XMM15,
    #endregion
    // Flags register
    RFLAGS = 0xff,
    // Instruction pointer
    RIP,

    IDTR,
    GDTR,
}