using HypotheticalComputingMachineClassLib.HCMModel_D1;
using HypotheticalComputingMachineClassLib.HypotheticalComputingMachineModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypotheticalComputingMachineClassLib.DICodeInterpreter.Commands
{
    public class AndCommand : LogicalBinaryOperationD1Command
    {
        public AndCommand(HypotheticalComputingMachineD1Model hcm, byte[] copBytes) 
            : base(hcm, copBytes) { return; }

        public override byte[] BinaryOperation(byte[] op1, byte[] op2)
        {
            return BinaryOperators.And(op1, op2);
        }
    }
}
