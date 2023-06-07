using HypotheticalComputingMachineClassLib.HCMModel_D1;
using HypotheticalComputingMachineClassLib.HypotheticalComputingMachineModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypotheticalComputingMachineClassLib.DICodeInterpreter.Commands
{
    public class CallCommand : D1Command
    {
        public CallCommand(HypotheticalComputingMachineD1Model hcm, byte[] copBytes)
            : base(hcm, copBytes) { return; }

        public override void ExecuteImpl()
        {
            BuildExecutionAddresses();

            WriteOperandInMemory(3, HCM.CS.SAK);
            HCM.CS.SAK = new byte[] { CopBytes[1] };

            return;
        }
    }
}
