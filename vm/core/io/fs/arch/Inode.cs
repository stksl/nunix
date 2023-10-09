
using System.Runtime.InteropServices;

namespace Nunix.IO;
public struct Inode 
{
    public ulong[] m_data {get; private set;}
    
    public Inode(byte[] bytes, int index)
    {
        m_data = new ulong[2];
        m_data[0] = BitConverter.ToUInt64(bytes, index);
        m_data[1] = BitConverter.ToUInt64(bytes, index + 8);
    }
    /// <summary>
    /// Offset has to be within 1-5 range (see inode structure in info.txt) 
    /// </summary>
    /// <typeparam name="TNum"></typeparam>
    /// <param name="offset"></param>
    /// <returns></returns>
    public ulong GetPart(int offset) 
    {
        ulong shifted = m_data[1];
        ulong mask = 0;
        switch(offset) 
        {
            case 1:
                mask = 0xff_ff_ff_ff;
                break;
            case 2:
                shifted >>= 32;
                break;
            case 3:
                shifted = m_data[0];
                mask = 0xff_ff_ff;
                break;
            case 4:
                shifted = m_data[0] >> 24;
                mask = 0xff_ff;
                break;
            case 5:
                shifted = m_data[0] >> 40;
                mask = 0xff_ff_ff;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(offset));
        }
        return shifted & mask;
    }
}