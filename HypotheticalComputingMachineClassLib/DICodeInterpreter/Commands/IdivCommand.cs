using HypotheticalComputingMachineClassLib.HCMModel_D1;
using HypotheticalComputingMachineClassLib.HypotheticalComputingMachineModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypotheticalComputingMachineClassLib.DICodeInterpreter.Commands
{
    public class IdivCommand : ArithmeticBinaryOperationD1Command
    {
        public IdivCommand(HypotheticalComputingMachineD1Model hcm, byte[] copBytes)
            : base(hcm, copBytes) { return; }

        protected override void SaveAccumulatorAndSetRPImpl(byte[] data, bool[] modifyRP)
        {
            if (!modifyRP[2])
            {
                base.SaveAccumulatorAndSetRPImpl(data, modifyRP);
            }
            else
            {
                modifyRP[0] = false;
                modifyRP[1] = false;
                Halt();
            }

            return;
        }

        public override byte[] BinaryOperation(byte[] op1, byte[] op2)
        {
            byte[] res = null;

            try
            {
                (res, _) = BinaryOperators.Idiv(op1, op2);
            }
            catch
            {
                res = new byte[4];
                _operationFault = true;
            }

            return res;
        }

        protected override void ProcessResult()
        {
            if (!HCM.CS.RP[2])
            {
                base.ProcessResult();
            }

            return;
        }
    }
}

