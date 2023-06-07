using HypotheticalComputingMachineClassLib.HCMModel_D1;
using HypotheticalComputingMachineClassLib.HypotheticalComputingMachineModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypotheticalComputingMachineClassLib.DICodeInterpreter.Commands
{
    public class SalCommand : ShiftingOperationD1Command
    {
        public SalCommand(HypotheticalComputingMachineD1Model hcm, byte[] copBytes)
            : base(hcm, copBytes) { return; }

        public override void ExecuteImpl()
        {
            BuildExecutionAddresses();
            HCM.CS.OR1 = ReadOperandFromMemory(1);
            HCM.CS.OR2 = new byte[] { CopBytes[2] };
            byte[] op3Bytes = null;
            int shiftTimes = CopBytes[2];

            if (shiftTimes > 31)
                op3Bytes = new byte[4];
            else
            {
                op3Bytes = BinaryOperators.Sal(HCM.CS.OR1, shiftTimes);
            }
            WriteOperandInMemory(3, op3Bytes);
            SaveAccumulatorAndSetRP(op3Bytes, new bool[] { true, true, false, false });

            return;
        }
    }
}
