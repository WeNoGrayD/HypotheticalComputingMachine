using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HypotheticalComputingMachineClassLib.CodeInterpreter
{
    public abstract class CodeInterpreter<THypotheticalComputingMachineModel>
    {
        public IHypotheticalComputingMachineCommand<THypotheticalComputingMachineModel> Interprete(byte[] copBytes)
        {
            CheckErrors(copBytes);

            (COP cop, copBytes) = RecognizeCop(copBytes);

            return InterpreteCop(cop, copBytes);
        }

        protected virtual void CheckErrors(byte[] copBytes)
        {
            if (copBytes.Length != 4)
                throw new InvalidOperationException("32 bit must be command");

            return;
        }

        protected virtual (COP Cop, byte[] CopBytes) RecognizeCop(byte[] copBytes)
        {
            COP cop = (COP)(copBytes[0] >> 4);
            byte[] copBytesWithNoCop = new byte[4];
            copBytes.CopyTo(copBytesWithNoCop, 0);
            copBytesWithNoCop[0] &= 0b00001111;

            return (cop, copBytesWithNoCop);
        }

        protected abstract IHypotheticalComputingMachineCommand<THypotheticalComputingMachineModel>
            InterpreteCop(COP cop, byte[] copBytes);
    }
}
