namespace Nunix.Core;
public interface IRegister 
{
    int Size {get;}
    bool IsGeneralPurpose {get;}
    RegType RegType {get;}

}