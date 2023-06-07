using HypotheticalComputingMachineClassLib.CodeInterpreter;
using HypotheticalComputingMachineClassLib.DICodeInterpreter.Commands;
using HypotheticalComputingMachineClassLib.HCMModel_D1;
using HypotheticalComputingMachineClassLib.HypotheticalComputingMachineModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypotheticalComputingMachineClassLib.DICodeInterpreter
{
    public abstract class D1Command : IHypotheticalComputingMachineCommand<HypotheticalComputingMachineD1Model>
    {
        public HypotheticalComputingMachineD1Model HCM { get; private set; }

        public byte[] CopBytes { get; private set; }

        public byte M
        {
            get
            {
                return CopBytes[0];
            }
        }

        public D1Command(HypotheticalComputingMachineD1Model hcm, byte[] copBytes)
        {
            HCM = hcm;
            CopBytes = new byte[4]; 
            copBytes.CopyTo(CopBytes, 0);
        }

        public void BuildExecutionAddresses()
        {
            if (this.M == 0) return;

            byte[] modificationCellData = HCM.Cache.ReadFromMemCell(this.M);

            for (int i = 1; i <= 3; i++)
                CopBytes[i] += modificationCellData[i];

            HCM.CS.RK = CopBytes;

            return;
        }

        public byte[] ReadOperandFromMemory(int Ai)
        {
            int operandAddress = CopBytes[Ai];

            HCM.CS.RA = new byte[] { CopBytes[Ai] };

            return HCM.DS.ReadFromMemCell(operandAddress);
        }

        public void WriteOperandInMemory(int Ai, byte[] data)
        {
            int operandAddress = CopBytes[Ai];
            HCM.DS.WriteToMemCell(operandAddress, data);

            return;
        }

        public void SaveAccumulatorAndSetRP(byte[] data, bool[] modifyRP)
        {
            SaveAccumulatorAndSetRPImpl(data, modifyRP);
            HCM.CS.RP = modifyRP;

            return;
        }

        protected virtual void SaveAccumulatorAndSetRPImpl(byte[] data, bool[] modifyRP)
        {
            HCM.CS.ACC = data;
        }

        public void GotoNextCommand()
        {
            HCM.CS.SAK = HCM.CS.SAK.Increment();

            return;
        }

        public void Halt()
        {
            HCM.Program.EOF = true;

            return;
        }

        public void Execute()
        {
            GotoNextCommand();
            ExecuteImpl();

            return;
        }

        public abstract void ExecuteImpl();
    }
}
