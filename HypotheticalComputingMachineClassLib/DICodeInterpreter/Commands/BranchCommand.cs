using HypotheticalComputingMachineClassLib.HCMModel_D1;
using HypotheticalComputingMachineClassLib.HypotheticalComputingMachineModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypotheticalComputingMachineClassLib.DICodeInterpreter.Commands
{
    public class BranchCommand : D1Command
    {
        public BranchCommand(HypotheticalComputingMachineD1Model hcm, byte[] copBytes)
            : base(hcm, copBytes) { return; }

        public override void ExecuteImpl()
        {
            BuildExecutionAddresses();

            byte destinationAddress = (HCM.CS.RP[0], HCM.CS.RP[1]) switch
            {
                (true, _) => CopBytes[1],
                (_, true) => CopBytes[2],
                _ => CopBytes[3]
            };
            HCM.CS.SAK = new byte[] { destinationAddress };

            return;
        }
    }
}
