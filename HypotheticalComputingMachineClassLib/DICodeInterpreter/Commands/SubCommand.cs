using HypotheticalComputingMachineClassLib.HCMModel_D1;
using HypotheticalComputingMachineClassLib.HypotheticalComputingMachineModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypotheticalComputingMachineClassLib.DICodeInterpreter.Commands
{
    public class SubCommand : ArithmeticBinaryOperationD1Command
    {
        public SubCommand(HypotheticalComputingMachineD1Model hcm, byte[] copBytes)
            : base(hcm, copBytes) { return; }

        public override byte[] BinaryOperation(byte[] op1, byte[] op2)
        {
            (byte[] res, byte borrow) = BinaryOperators.Sub(op1, op2);
            _operationFault = borrow == 0b1;

            return res;
        }
    }
}
