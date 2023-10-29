using System.Text;
using System.Text.RegularExpressions;

namespace Nunix.IO;

public struct FullPath 
{
    public readonly LinkedList<FileName> Path;
    public FullPath(LinkedList<FileName> fullpath)
    {
        Path = fullpath;
    }

    public FullPath AppendLast(FileName filename) 
    {
        return new FullPath(new LinkedList<FileName>(Path).AddLast(filename).List!);
    }
    public FullPath RemoveLast() 
    {
        LinkedList<FileName> newpath = new LinkedList<FileName>(Path);
        newpath.RemoveLast();

        return new FullPath(newpath);
    } 
    
}