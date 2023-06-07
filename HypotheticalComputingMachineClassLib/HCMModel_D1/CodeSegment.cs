using HypotheticalComputingMachineClassLib.DICodeInterpreter;
using HypotheticalComputingMachineClassLib.HypotheticalComputingMachineModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HypotheticalComputingMachineClassLib.CodeInterpreter;

namespace HypotheticalComputingMachineClassLib.HCMModel_D1
{
    public interface IHCMCodeSegment
    {
        byte[] PA { get; set; }

        byte[] SAK { get; set; }

        byte[] RA { get; set; }

        byte[] RS { get; set; }

        byte[] RK { get; set; }

        byte[] OR1 { get; set; }

        byte[] OR2 { get; set; }

        byte[] ACC { get; set; }

        bool[] RP { get; set; }

        void Reset();

        void SoftReset();

        void StartCSFrom(byte startAddress);

        void LinkToProgram<THCM>(IHCMProgram<THCM> program)
            where THCM : IHypotheticalComputingMachineModel;
    }

    public abstract class CodeSegmentBase : IHCMCodeSegment
    {
        #region Instance fields

        protected IHCMRegister<byte> _pa;

        protected IHCMRegister<byte> _sak;

        protected IHCMRegister<byte> _ra;

        protected IHCMRegister<byte> _rs;

        protected IHCMRegister<byte> _rk;

        protected IHCMRegister<byte> _or1;

        protected IHCMRegister<byte> _or2;

        protected IHCMRegister<byte> _acc;

        protected IHCMRegister<bool> _rp;

        #endregion

        public virtual byte[] PA { get => _pa.ToArray(); set => value.ShiftIn(_pa); }

        public virtual byte[] SAK { get => _sak.ToArray(); set => value.ShiftIn(_sak); }

        public virtual byte[] RA { get => _ra.ToArray(); set => value.ShiftIn(_ra); }

        public virtual byte[] RS { get => _rs.ToArray(); set => value.ShiftIn(_rs); }

        public virtual byte[] RK { get => _rk.ToArray(); set => value.ShiftIn(_rk); }

        public virtual byte[] OR1 { get => _or1.ToArray(); set => value.ShiftIn(_or1); }

        public virtual byte[] OR2 { get => _or2.ToArray(); set => value.ShiftIn(_or2); }

        public virtual byte[] ACC { get => _acc.ToArray(); set => value.ShiftIn(_acc); }

        public virtual bool[] RP { get => _rp.ToArray(); set => value.ShiftIn(_rp); }

        #region Constructors

        /// <summary>
        /// Конструктор.
        /// </summary>
        public CodeSegmentBase()
        {
            return;
        }

        #endregion

        public void Reset()
        {
            _pa.Reset();
            SoftReset();

            return;
        }

        public virtual void SoftReset()
        {
            SAK = PA;
            _sak.SoftReset();
            _ra.Reset();
            _rs.Reset();
            _rk.Reset();
            _or1.Reset();
            _or2.Reset();
            _acc.Reset();
            _rp.Reset();

            return;
        }

        public void StartCSFrom(byte startAddress)
        {
            PA = BinaryInOutConverter.Int32ToBytes(startAddress);

            return;
        }

        public virtual void LinkToProgram<THCM>(IHCMProgram<THCM> program)
            where THCM : IHypotheticalComputingMachineModel
        { return; }
    }

    /// <summary>
    /// Класс, содержащий информацию о сегменте кода.
    /// </summary>
    public class D1CodeSegment : CodeSegmentBase
    {
        #region Constructors

        /// <summary>
        /// Конструктор.
        /// </summary>
        public D1CodeSegment()
        {
            _pa = new ArrayRegister<byte>(1);
            _sak = new ArrayRegister<byte>(1);
            _ra = new ArrayRegister<byte>(1);
            _rs = new ArrayRegister<byte>(4);
            _rk = new ArrayRegister<byte>(4);
            _or1 = new ArrayRegister<byte>(4);
            _or2 = new ArrayRegister<byte>(4);
            _acc = new ArrayRegister<byte>(4);
            _rp = new ArrayRegister<bool>(4);

            return;
        }

        #endregion
    }
}
