using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypotheticalComputingMachineClassLib
{
    public enum COP : byte
    {
        Halt = 0b0000,
        Add = 0b0001,
        Sub = 0b0010,
        Imul = 0b0011,
        Idiv = 0b0100,
        And = 0b0101,
        Or = 0b0110,
        Sal = 0b0111,
        Sar = 0b1000,
        Ret = 0b1001,
        Call = 0b1010,
        Branch = 0b1011,
        Jmp = 0b1100,
        EndWhile = 0b1101,
        SaveCache = 0b1110,
        LoadToCache = 0b1111
    }
}
