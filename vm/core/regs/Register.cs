namespace Nunix.Core;

public class Register : IRegister
{
    public int Size => size;
    private readonly int size;
    public bool IsGeneralPurpose => isGeneralPurpose;
    private readonly bool isGeneralPurpose;
    public RegType RegType => regType;
    private readonly RegType regType;
    public RegisterValue Value { get; protected set; }
#pragma warning disable 8618
    internal Register(int size, bool generalPurpose, RegType regType, bool allocVal)
    {
        this.size = size;
        this.isGeneralPurpose = generalPurpose;
        this.regType = regType;

        if (allocVal)
            Value = new RegisterValue(this);
    }
#pragma warning restore
    public static int GetSize(RegType regType)
    {
        if (regType == RegType.Unkown) return (int)regType;

        switch (regType)
        {
            case RegType.RIP:
            case RegType.RFLAGS:
            case >= RegType.RAX and <= RegType.R15:
                return 8;
            case >= RegType.EAX and <= RegType.R15d:
                return 4;
            case >= RegType.AX and <= RegType.R15w:
                return 2;
            case >= RegType.AL and <= RegType.R15b:
                return 1;

        }

        return 16;
    }
}
public sealed class GPRegister : Register
{
    internal GPRegister(int size, RegType regType) : base(size, true, regType, false)
    {
        if (regType < RegType.RAX || regType > RegType.R15b)
        {
            throw new ArgumentException("Not a general-purpose register");
        }


        Value = new RegisterValue(this);
    }
    public RegType? GetUpper() 
    {
        RegType type = RegType - 0x10;
        return Enum.IsDefined(type) ? type : null;
    }
    public RegType? GetLower() 
    {
        RegType type = RegType + 0x10;
        return Enum.IsDefined(type) ? type : null;
    }
}