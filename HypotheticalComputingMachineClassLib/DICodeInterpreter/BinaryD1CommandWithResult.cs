using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HypotheticalComputingMachineClassLib.HypotheticalComputingMachineModel;
using HypotheticalComputingMachineClassLib.HCMModel_D1;

namespace HypotheticalComputingMachineClassLib.DICodeInterpreter
{
    public abstract class BinaryOperationD1CommandWithResult : D1Command
    {
        protected bool _operationFault = false;

        public BinaryOperationD1CommandWithResult(HypotheticalComputingMachineD1Model hcm, byte[] copBytes)
            : base(hcm, copBytes)
        {
            return;
        }

        public override void ExecuteImpl()
        {
            BuildExecutionAddresses();

            HCM.CS.OR1 = ReadOperandFromMemory(1);
            HCM.CS.OR2 = ReadOperandFromMemory(2);

            SaveAccumulatorAndSetRP(BinaryOperation(HCM.CS.OR1, HCM.CS.OR2), new bool[] { true, true, _operationFault, false });
            ProcessResult();

            return;
        }

        public abstract byte[] BinaryOperation(byte[] op1, byte[] op2);

        protected virtual void ProcessResult()
        {
            WriteOperandInMemory(3, HCM.CS.ACC);

            return;
        }
    }

    public abstract class ArithmeticBinaryOperationD1Command : BinaryOperationD1CommandWithResult
    {
        public ArithmeticBinaryOperationD1Command(HypotheticalComputingMachineD1Model hcm, byte[] copBytes)
            : base(hcm, copBytes)
        {
            return;
        }

        protected override void SaveAccumulatorAndSetRPImpl(byte[] data, bool[] modifyRP)
        {
            base.SaveAccumulatorAndSetRPImpl(data, modifyRP);

            if (modifyRP[0]) modifyRP[0] = (sbyte)HCM.CS.ACC[0] < 0;
            if (modifyRP[1]) modifyRP[1] = BinaryInOutConverter.BytesToInt32(HCM.CS.ACC) == 0;

            return;
        }
    }

    public abstract class LogicalBinaryOperationD1Command : BinaryOperationD1CommandWithResult
    {
        public LogicalBinaryOperationD1Command(HypotheticalComputingMachineD1Model hcm, byte[] copBytes)
            : base(hcm, copBytes)
        {
            return;
        }

        protected override void SaveAccumulatorAndSetRPImpl(byte[] data, bool[] modifyRP)
        {
            base.SaveAccumulatorAndSetRPImpl(data, modifyRP);

            if (modifyRP[0]) modifyRP[0] = HCM.CS.ACC.Any(b => b != 0);
            if (modifyRP[1]) modifyRP[1] = !modifyRP[0];

            return;
        }
    }

    public abstract class ShiftingOperationD1Command : D1Command
    {
        public ShiftingOperationD1Command(HypotheticalComputingMachineD1Model hcm, byte[] copBytes)
            : base(hcm, copBytes)
        {
            return;
        }

        protected override void SaveAccumulatorAndSetRPImpl(byte[] data, bool[] modifyRP)
        {
            base.SaveAccumulatorAndSetRPImpl(data, modifyRP);

            if (modifyRP[0]) modifyRP[0] = HCM.CS.ACC.Any(b => b != 0);
            if (modifyRP[1]) modifyRP[1] = !modifyRP[0];

            return;
        }
    }
}
