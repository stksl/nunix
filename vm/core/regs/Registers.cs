using System.Reflection;

namespace Nunix.Core;
public static class Registers 
{
    internal static Dictionary<RegType, Register> regsByTypes;
    #region General-Purpose 
    public static readonly Register RAX;
    public static readonly Register RBX;
    public static readonly Register RCX;
    public static readonly Register RDX;
    public static readonly Register RDI;
    public static readonly Register RSI;
    public static readonly Register RSP;
    public static readonly Register RBP;
    public static readonly Register R8;
    public static readonly Register R9;
    public static readonly Register R10;
    public static readonly Register R11;
    public static readonly Register R12;
    public static readonly Register R13;
    public static readonly Register R14;
    public static readonly Register R15;
    // 32 lower bits
    public static readonly Register EAX;
    public static readonly Register EBX;
    public static readonly Register ECX;
    public static readonly Register EDX;
    public static readonly Register EDI;
    public static readonly Register ESI;
    public static readonly Register ESP;
    public static readonly Register EBP;
    public static readonly Register R8d;
    public static readonly Register R9d;
    public static readonly Register R10d;
    public static readonly Register R11d;
    public static readonly Register R12d;
    public static readonly Register R13d;
    public static readonly Register R14d;
    public static readonly Register R15d;
    // lower 16 bits
    public static readonly Register AX;
    public static readonly Register BX;
    public static readonly Register CX;
    public static readonly Register DX;
    public static readonly Register DI;
    public static readonly Register SI;
    public static readonly Register SP;
    public static readonly Register BP;
    public static readonly Register R8w;
    public static readonly Register R9w;
    public static readonly Register R10w;
    public static readonly Register R11w;
    public static readonly Register R12w;
    public static readonly Register R13w;
    public static readonly Register R14w;
    public static readonly Register R15w;
    // lower 8 bits
    public static readonly Register AL;
    public static readonly Register BL;
    public static readonly Register CL;
    public static readonly Register DL;
    public static readonly Register DIL;
    public static readonly Register SIL;
    public static readonly Register SPL;
    public static readonly Register BPL;
    public static readonly Register R8b;
    public static readonly Register R9b;
    public static readonly Register R10b;
    public static readonly Register R11b;
    public static readonly Register R12b;
    public static readonly Register R13b;
    public static readonly Register R14b;
    public static readonly Register R15b;
    #endregion
    static Registers()
    {
        #region gp_regs_init
        RAX = new GPRegister((int)AsmSize.Qword, RegType.RAX);
        RBX = new GPRegister((int)AsmSize.Qword, RegType.RBX);
        RCX = new GPRegister((int)AsmSize.Qword, RegType.RCX);
        RDX = new GPRegister((int)AsmSize.Qword, RegType.RDX);
        RDI = new GPRegister((int)AsmSize.Qword, RegType.RDI);
        RSI = new GPRegister((int)AsmSize.Qword, RegType.RSI);
        RSP = new GPRegister((int)AsmSize.Qword, RegType.RSP);
        RBP = new GPRegister((int)AsmSize.Qword, RegType.RBP);
        R8 = new GPRegister((int)AsmSize.Qword, RegType.R8);
        R9 = new GPRegister((int)AsmSize.Qword, RegType.R9);
        R10 = new GPRegister((int)AsmSize.Qword, RegType.R10);
        R11 = new GPRegister((int)AsmSize.Qword, RegType.R11);
        R12 = new GPRegister((int)AsmSize.Qword, RegType.R12);
        R13 = new GPRegister((int)AsmSize.Qword, RegType.R13);
        R14 = new GPRegister((int)AsmSize.Qword, RegType.R14);
        R15 = new GPRegister((int)AsmSize.Qword, RegType.R15);

        EAX = new GPRegister((int)AsmSize.Dword, RegType.EAX);
        EBX = new GPRegister((int)AsmSize.Dword, RegType.EBX);
        ECX = new GPRegister((int)AsmSize.Dword, RegType.ECX);
        EDX = new GPRegister((int)AsmSize.Dword, RegType.EDX);
        EDI = new GPRegister((int)AsmSize.Dword, RegType.EDI);
        ESI = new GPRegister((int)AsmSize.Dword, RegType.ESI);
        ESP = new GPRegister((int)AsmSize.Dword, RegType.ESP);
        EBP = new GPRegister((int)AsmSize.Dword, RegType.EBP);
        R8d = new GPRegister((int)AsmSize.Dword, RegType.R8d);
        R9d = new GPRegister((int)AsmSize.Dword, RegType.R9d);
        R10d = new GPRegister((int)AsmSize.Dword, RegType.R10d);
        R11d = new GPRegister((int)AsmSize.Dword, RegType.R11d);
        R12d = new GPRegister((int)AsmSize.Dword, RegType.R12d);
        R13d = new GPRegister((int)AsmSize.Dword, RegType.R13d);
        R14d = new GPRegister((int)AsmSize.Dword, RegType.R14d);
        R15d = new GPRegister((int)AsmSize.Dword, RegType.R15d);

        AX = new GPRegister((int)AsmSize.Word, RegType.AX);
        BX = new GPRegister((int)AsmSize.Word, RegType.BX);
        CX = new GPRegister((int)AsmSize.Word, RegType.CX);
        DX = new GPRegister((int)AsmSize.Word, RegType.DX);
        DI = new GPRegister((int)AsmSize.Word, RegType.DI);
        SI = new GPRegister((int)AsmSize.Word, RegType.SI);
        SP = new GPRegister((int)AsmSize.Word, RegType.SP);
        BP = new GPRegister((int)AsmSize.Word, RegType.BP);
        R8w = new GPRegister((int)AsmSize.Word, RegType.R8w);
        R9w = new GPRegister((int)AsmSize.Word, RegType.R9w);
        R10w = new GPRegister((int)AsmSize.Word, RegType.R10w);
        R11w = new GPRegister((int)AsmSize.Word, RegType.R11w);
        R12w = new GPRegister((int)AsmSize.Word, RegType.R12w);
        R13w = new GPRegister((int)AsmSize.Word, RegType.R13w);
        R14w = new GPRegister((int)AsmSize.Word, RegType.R14w);
        R15w = new GPRegister((int)AsmSize.Word, RegType.R15w);

        AL = new GPRegister((int)AsmSize.Byte, RegType.AL);
        BL = new GPRegister((int)AsmSize.Byte, RegType.BL);
        CL = new GPRegister((int)AsmSize.Byte, RegType.CL);
        DL = new GPRegister((int)AsmSize.Byte, RegType.DL);
        DIL = new GPRegister((int)AsmSize.Byte, RegType.DIL);
        SIL = new GPRegister((int)AsmSize.Byte, RegType.SIL);
        SPL = new GPRegister((int)AsmSize.Byte, RegType.SPL);
        BPL = new GPRegister((int)AsmSize.Byte, RegType.BPL);
        R8b = new GPRegister((int)AsmSize.Byte, RegType.R8b);
        R9b = new GPRegister((int)AsmSize.Byte, RegType.R9b);
        R10b = new GPRegister((int)AsmSize.Byte, RegType.R10b);
        R11b = new GPRegister((int)AsmSize.Byte, RegType.R11b);
        R12b = new GPRegister((int)AsmSize.Byte, RegType.R12b);
        R13b = new GPRegister((int)AsmSize.Byte, RegType.R13b);
        R14b = new GPRegister((int)AsmSize.Byte, RegType.R14b);
        R15b = new GPRegister((int)AsmSize.Byte, RegType.R15b);
        #endregion
        
        // maybe refactor later, basically going through all of the regs fields via reflection
        regsByTypes = new Dictionary<RegType, Register>();
        FieldInfo[] fields = typeof(Registers).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
        foreach(FieldInfo field in fields) 
        {
            Register reg = (Register)field.GetValue(null)!;
            // initializing register values right now, even if its a general-purpose register 
            // GetUpper() returns already present register in regsByTypes hashmap
            reg.value.Init();

            regsByTypes[reg.RegType] = reg;
        }
    } 
    public static Register GetRegister(RegType type) 
    {
        return regsByTypes[type];
    }
}