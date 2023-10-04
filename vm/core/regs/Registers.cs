using System.Reflection;

namespace Nunix.Core;
public static class Registers 
{
    private static Dictionary<RegType, Register> regsByTypes;
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
        RAX = new GPRegister(8, RegType.RAX);
        RBX = new GPRegister(8, RegType.RBX);
        RCX = new GPRegister(8, RegType.RCX);
        RDX = new GPRegister(8, RegType.RDX);
        RDI = new GPRegister(8, RegType.RDI);
        RSI = new GPRegister(8, RegType.RSI);
        RSP = new GPRegister(8, RegType.RSP);
        RBP = new GPRegister(8, RegType.RBP);
        R8 = new GPRegister(8, RegType.R8);
        R9 = new GPRegister(8, RegType.R9);
        R10 = new GPRegister(8, RegType.R10);
        R11 = new GPRegister(8, RegType.R11);
        R12 = new GPRegister(8, RegType.R12);
        R13 = new GPRegister(8, RegType.R13);
        R14 = new GPRegister(8, RegType.R14);
        R15 = new GPRegister(8, RegType.R15);

        EAX = new GPRegister(4, RegType.EAX);
        EBX = new GPRegister(4, RegType.EBX);
        ECX = new GPRegister(4, RegType.ECX);
        EDX = new GPRegister(4, RegType.EDX);
        EDI = new GPRegister(4, RegType.EDI);
        ESI = new GPRegister(4, RegType.ESI);
        ESP = new GPRegister(4, RegType.ESP);
        EBP = new GPRegister(4, RegType.EBP);
        R8d = new GPRegister(4, RegType.R8d);
        R9d = new GPRegister(4, RegType.R9d);
        R10d = new GPRegister(4, RegType.R10d);
        R11d = new GPRegister(4, RegType.R11d);
        R12d = new GPRegister(4, RegType.R12d);
        R13d = new GPRegister(4, RegType.R13d);
        R14d = new GPRegister(4, RegType.R14d);
        R15d = new GPRegister(4, RegType.R15d);

        AX = new GPRegister(2, RegType.AX);
        BX = new GPRegister(2, RegType.BX);
        CX = new GPRegister(2, RegType.CX);
        DX = new GPRegister(2, RegType.DX);
        DI = new GPRegister(2, RegType.DI);
        SI = new GPRegister(2, RegType.SI);
        SP = new GPRegister(2, RegType.SP);
        BP = new GPRegister(2, RegType.BP);
        R8w = new GPRegister(2, RegType.R8w);
        R9w = new GPRegister(2, RegType.R9w);
        R10w = new GPRegister(2, RegType.R10w);
        R11w = new GPRegister(2, RegType.R11w);
        R12w = new GPRegister(2, RegType.R12w);
        R13w = new GPRegister(2, RegType.R13w);
        R14w = new GPRegister(2, RegType.R14w);
        R15w = new GPRegister(2, RegType.R15w);

        AL = new GPRegister(1, RegType.AL);
        BL = new GPRegister(1, RegType.BL);
        CL = new GPRegister(1, RegType.CL);
        DL = new GPRegister(1, RegType.DL);
        DIL = new GPRegister(1, RegType.DIL);
        SIL = new GPRegister(1, RegType.SIL);
        SPL = new GPRegister(1, RegType.SPL);
        BPL = new GPRegister(1, RegType.BPL);
        R8b = new GPRegister(1, RegType.R8b);
        R9b = new GPRegister(1, RegType.R9b);
        R10b = new GPRegister(1, RegType.R10b);
        R11b = new GPRegister(1, RegType.R11b);
        R12b = new GPRegister(1, RegType.R12b);
        R13b = new GPRegister(1, RegType.R13b);
        R14b = new GPRegister(1, RegType.R14b);
        R15b = new GPRegister(1, RegType.R15b);
        #endregion
        
        regsByTypes = new Dictionary<RegType, Register>();
        FieldInfo[] fields = typeof(Registers).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
        foreach(FieldInfo field in fields) 
        {
            Register reg = (Register)field.GetValue(null)!;
            // initializing register values right now, even if its a general-purpose register 
            // GetUpper() returns already present register in regsByTypes hashmap
            reg.Value.Init();

            regsByTypes[reg.RegType] = reg;
        }
    } 
    public static Register GetRegister(RegType type) 
    {
        return regsByTypes[type];
    }
}