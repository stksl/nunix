using System.Numerics;

namespace Nunix.Core;
public class Register : IRegister
{
    public int Size => size;
    private readonly int size;
    public bool IsGeneralPurpose => isGeneralPurpose;
    private readonly bool isGeneralPurpose;
    public RegType RegType => regType;
    private readonly RegType regType;
    /// <summary>
    /// not really recommended to use the prop out of the class, only for .Init()  
    /// </summary>
    internal RegisterValue value {get; init;}
#pragma warning disable 8618
    internal Register(int size, bool generalPurpose, RegType regType, bool allocVal)
    {
        this.size = size;
        this.isGeneralPurpose = generalPurpose;
        this.regType = regType;

        if (allocVal)
            value = new RegisterValue(this);
    }
#pragma warning restore
    /// <summary>
    /// Loads bytes value using little-endian
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="index"></param>
    /// <param name="length"></param>
    /// <param name="offset"></param>
    public unsafe void LoadValue(byte[] bytes, int index) 
    {
        // after that supposed to catch the exception and dispose registers
        if (bytes.Length - index != size)
            throw new ArgumentException($"Unable to load {size} byte register value in {bytes.Length - index} bytes");

        fixed(byte* ptr = bytes) 
        {
            byte* rawPtr = ptr + index;
            value.Handler.LoadRawValue(rawPtr, size);
        }
    }
    /// <summary>
    /// Loads the <paramref name="val_in"/> value into the register.
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <param name="val_in"></param>
    /// <exception cref="ArgumentException"></exception>
    public unsafe void LoadValue<TIn>(TIn val_in) where TIn : unmanaged, INumber<TIn> 
    {
        // after that supposed to catch the exception and dispose registers
        if(sizeof(TIn) != size) 
            throw new ArgumentException($"Unable to load {size} byte register value in {sizeof(TIn)} bytes");

        value.Handler.LoadRawValue(&val_in, sizeof(TIn));
    }
    /// <summary>
    /// Load full register value into <paramref name="bytes"/> (little-endian)
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="index"></param>
    public unsafe void GetValue(byte[] bytes, int index) 
    {
        for(int offset = 0; offset < size; offset++) 
        {
            bytes[index + offset] = *value.Handler.GetHandle(offset);
        }
    }
    /// <summary>
    /// Load register value in <paramref name="out_"/> fully or partially (big-endian)
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    /// <param name="out_"></param>
    public unsafe void GetValue<TOut>(TOut* out_) where TOut : unmanaged, INumber<TOut>
    {
        // for example RAX but "TOut == int", will get last 4 bytes, works only for getters  
        byte* ptr = (byte*)out_;
        for(int offset = 0; offset < sizeof(TOut); offset++) 
        {
            *(ptr + offset) = *value.Handler.GetHandle(offset);
        }
    }

}
// General-purpose register
public sealed class GPRegister : Register
{
    internal GPRegister(int size, RegType regType) : base(size, true, regType, false)
    {
        if (regType < RegType.RAX || regType > RegType.R15b)
        {
            throw new ArgumentException("Not a general-purpose register");
        }

        value = new RegisterValue(this);
    }

    // get upper register type
    public RegType? GetUpper() 
    {
        RegType type = RegType - 0x10;
        return Enum.IsDefined(type) ? type : null;
    }
    // get lower register type
    public RegType? GetLower() 
    {
        RegType type = RegType + 0x10;
        return Enum.IsDefined(type) ? type : null;
    }
}