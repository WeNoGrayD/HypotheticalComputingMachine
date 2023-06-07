using HypotheticalComputingMachineClassLib.HCMModel_D1;
using HypotheticalComputingMachineClassLib.HypotheticalComputingMachineModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypotheticalComputingMachineClassLib.DICodeInterpreter.Commands
{
    public class EndWhileCommand : D1Command
    {
        public EndWhileCommand(HypotheticalComputingMachineD1Model hcm, byte[] copBytes)
            : base(hcm, copBytes) { return; }

        public override void ExecuteImpl()
        {
            byte[] cacheData = HCM.Cache.ReadFromMemCell(this.M),
                   redirectionConsts = ReadOperandFromMemory(1);

            HCM.CS.OR1 = cacheData;
            HCM.CS.OR2 = new byte[4] { 255, redirectionConsts[1], redirectionConsts[2], redirectionConsts[3] };

            cacheData[0]--;
            for (int i = 1; i <= 3; i++)
                cacheData[i] += redirectionConsts[i];
            SaveAccumulatorAndSetRP(cacheData, new bool[] { false, false, false, (sbyte)cacheData[0] < 0 });

            if (this.HCM.CS.RP[3])
            {
                HCM.CS.SAK = new byte[] { CopBytes[3] };
            }
            else
            {
                HCM.CS.SAK = new byte[] { CopBytes[2] };
                HCM.Cache.WriteToMemCell(this.M, cacheData);
            }

            return;
        }
    }
}
