using HypotheticalComputingMachineClassLib.HypotheticalComputingMachineModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypotheticalComputingMachineClassLib.HCMModel_D1
{
    public interface IHCMDataSegment
    {
        int Length { get; }

        void Reset();

        void SoftReset();

        void WriteToMemCell(int address, byte[] data, bool makeSideEffects = true);

        byte[] ReadFromMemCell(int address, bool makeSideEffects = true);

        void LinkToProgram<THCM>(IHCMProgram<THCM> program)
            where THCM : IHypotheticalComputingMachineModel;
    }

    public abstract class DataSegmentBase : IHCMDataSegment
    {
        #region Fields

        protected IHypotheticalComputingMachineModel _hcm;

        protected IHCMRegister<byte>[] _memory;

        #endregion

        public byte[] this[int address]
        {
            get => _memory[address].ToArray();
            set => value.ShiftIn(_memory[address]);
        }

        public int Length { get => _memory.Length; }

        public DataSegmentBase(
            IHypotheticalComputingMachineModel hcm, 
            int memoryCellCount, 
            int memoryCellBytesCount,
            Func<int, int, IHCMRegister<byte>> registerFactory)
        {
            _hcm = hcm;
            _memory = new IHCMRegister<byte>[memoryCellCount];
            for (int i = 0; i < memoryCellCount; i++)
                _memory[i] = registerFactory(i, memoryCellBytesCount);
        }

        public void Reset()
        {
            foreach (IHCMRegister<byte> reg in _memory) reg.Reset();
            SoftReset();

            return;
        }

        public void SoftReset()
        {
            foreach (var reg in _memory)
                reg.SoftReset();

            return;
        }

        public virtual void WriteToMemCell(int address, byte[] data, bool makeSideEffects = true)
        {
            if (makeSideEffects)
            {
                _hcm.CS.RA = BinaryInOutConverter.Int32ToBytes(address);
                _hcm.CS.RS = data;
                this[BinaryInOutConverter.BytesToInt32(_hcm.CS.RA)] = _hcm.CS.RS;
            }
            else this[address] = data;

            return;
        }

        public virtual byte[] ReadFromMemCell(int address, bool makeSideEffects = true)
        {
            if (makeSideEffects)
            {
                _hcm.CS.RA = BinaryInOutConverter.Int32ToBytes(address);
                _hcm.CS.RS = this[BinaryInOutConverter.BytesToInt32(_hcm.CS.RA)];

                return _hcm.CS.RS;
            }
            else return this[address];
        }

        public virtual void LinkToProgram<THCM>(IHCMProgram<THCM> program)
            where THCM : IHypotheticalComputingMachineModel
        { return; }
    }

    public class D1DataSegment : DataSegmentBase
    {
        public D1DataSegment(IHypotheticalComputingMachineModel hcm)
            : base(hcm, 256, 4, (_, memCellBytesCount) => new ArrayRegister<byte>(memCellBytesCount))
        {
            return;
        }
    }
}
