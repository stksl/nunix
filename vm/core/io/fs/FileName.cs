using System.ComponentModel;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using Nunix.Core;

namespace Nunix.IO;

public struct FileName : IEquatable<FileName> 
{
    public static readonly string forbidden_symbols = "@#$%^&*=+{}[]'\"\\/<>?;:";
    private byte[] rawBytes;
    public string Raw;
    private FileName(byte[] raw) 
    {
        rawBytes = raw;

        Raw = string.Join("", rawBytes.Select(i => i.ToString("x2")));
    }
    public bool Equals(FileName other) 
    {
        return Raw == other.Raw;
    }
    public override string ToString()
    {
        return Raw;
    }
    public static FileName Create(string filename) 
    {
        foreach(char forbidden_char in forbidden_symbols) 
        {
            if (filename.Contains(forbidden_char)) throw new FormatException("forbidden character usage");
        }

        byte[] raw = new byte[VMConsts.HASH_FILENAME_LENGTH];
        byte[] sha256 = SHA256.HashData(Encoding.UTF8.GetBytes(filename));
        Array.Copy(sha256, SHA256.HashSizeInBytes - raw.Length, raw, 0, raw.Length);

        return new FileName(raw);
    }
}