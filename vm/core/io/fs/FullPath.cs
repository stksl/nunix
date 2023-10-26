using System.Text;
using System.Text.RegularExpressions;

namespace Nunix.IO;

public struct FullPath 
{
    public readonly string Raw {get
    {
        var runner = Path;
        StringBuilder sb = new StringBuilder();
        while (runner != null) 
        {
            sb.Append(runner.Value.ToString());
            sb.Append('/');
            runner = runner.Next;
        }

        return sb.ToString();
    }}
    public readonly LinkedListNode<FileName> Path;
    public FullPath(LinkedListNode<FileName> fullpath)
    {
        Path = fullpath;
    }
}