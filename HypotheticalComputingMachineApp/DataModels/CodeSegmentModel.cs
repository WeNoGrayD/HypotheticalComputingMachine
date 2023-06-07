using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HypotheticalComputingMachineClassLib;
using HypotheticalComputingMachineClassLib.CodeInterpreter;
using HypotheticalComputingMachineClassLib.DICodeInterpreter;
using HypotheticalComputingMachineClassLib.HCMModel_D1;
using HypotheticalComputingMachineClassLib.HypotheticalComputingMachineModel;

namespace HypotheticalComputingMachineApp.DataModels
{
    public class CodeSegmentModel<THCM> 
        : CodeSegmentBase,
          INotifyPropertyChanged
        where THCM : IHypotheticalComputingMachineModel
    {
        public IHCMRegister<byte> PARegister { get => _pa; }
        public IHCMRegister<byte> SAKRegister { get => _sak; }
        public IHCMRegister<byte> RARegister { get => _ra; }
        public IHCMRegister<byte> RSRegister { get => _rs; }
        public IHCMRegister<byte> RKRegister { get => _rk; }
        public IHCMRegister<byte> OR1Register { get => _or1; }
        public IHCMRegister<byte> OR2Register { get => _or2; }
        public IHCMRegister<byte> ACCRegister { get => _acc; }
        public IHCMRegister<bool> RPRegister { get => _rp; }

        #region Properties

        public override byte[] PA 
        { 
            get => base.PA;
            set
            {
                if (!Enumerable.SequenceEqual(_pa, value))
                {
                    base.PA = value;
                    OnPropertyChanged(nameof(PA));
                }
            }
        }

        /// <summary>
        /// Указатель на последнюю команду в этом сегменте.
        /// </summary>
        public override byte[] SAK
        {
            get => base.SAK;
            set
            {
                if (!Enumerable.SequenceEqual(_sak, value))
                {
                    base.SAK = value;
                    OnPropertyChanged(nameof(SAK));
                }
            }
        }

        public override byte[] RA
        {
            get => base.RA;
            set
            {
                if (!Enumerable.SequenceEqual(_ra, value))
                {
                    base.RA = value;
                    OnPropertyChanged(nameof(RA));
                }
            }
        }

        public override byte[] RS
        {
            get => base.RS;
            set
            {
                if (!Enumerable.SequenceEqual(_rs, value))
                {
                    base.RS = value;
                    OnPropertyChanged(nameof(RS));
                }
            }
        }

        public override byte[] RK
        {
            get => base.RK;
            set
            {
                if (!Enumerable.SequenceEqual(_rk, value))
                {
                    base.RK = value;
                    OnPropertyChanged(nameof(RK));
                }
            }
        }

        public override byte[] OR1
        {
            get => base.OR1;
            set
            {
                if (!Enumerable.SequenceEqual(_or1, value))
                {
                    base.OR1 = value;
                    OnPropertyChanged(nameof(OR1));
                }
            }
        }

        public override byte[] OR2
        {
            get => base.OR2;
            set
            {
                if (!Enumerable.SequenceEqual(_or2, value))
                {
                    base.OR2 = value;
                    OnPropertyChanged(nameof(OR2));
                }
            }
        }

        public override byte[] ACC
        {
            get => base.ACC;
            set
            {
                if (!Enumerable.SequenceEqual(_acc, value))
                {
                    base.ACC = value;
                    OnPropertyChanged(nameof(ACC));
                }
            }
        }

        public override bool[] RP
        {
            get => base.RP;
            set
            {
                if (!Enumerable.SequenceEqual(_rp, value))
                {
                    base.RP = value;
                    OnPropertyChanged(nameof(RP));
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Конструктор.
        /// </summary>
        public CodeSegmentModel()
            : base()
        {
            return;
        }

        #endregion

        #region Methods

        public override void SoftReset()
        {
            base.SoftReset();

            OnPropertyChanged(nameof(SAK));
            //OnPropertyChanged(nameof(SAKRegister));
            OnPropertyChanged(nameof(RA));
            //OnPropertyChanged(nameof(RARegister));
            OnPropertyChanged(nameof(RS));
            //OnPropertyChanged(nameof(RSRegister));
            OnPropertyChanged(nameof(RK));
            //OnPropertyChanged(nameof(RKRegister));
            OnPropertyChanged(nameof(OR1));
            //OnPropertyChanged(nameof(OR1Register));
            OnPropertyChanged(nameof(OR2));
            //OnPropertyChanged(nameof(OR2Register));
            OnPropertyChanged(nameof(ACC));
            //OnPropertyChanged(nameof(ACCRegister));
            OnPropertyChanged(nameof(RP));
            //OnPropertyChanged(nameof(RPRegister));
        }

        public override void LinkToProgram<THCM>(IHCMProgram<THCM> program)
        {
            _sak.LinkToProgram(program);
            _ra.LinkToProgram(program);
            _rs.LinkToProgram(program);
            _rk.LinkToProgram(program);
            _or1.LinkToProgram(program);
            _or2.LinkToProgram(program);
            _acc.LinkToProgram(program);
            _rp.LinkToProgram(program);

            return;
        }

        // Уведомление подписчиков на событие изменения свойства.

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #endregion
    }

    public class D1CodeSegmentModel : CodeSegmentModel<HypotheticalComputingMachineD1Model>
    {
        public D1CodeSegmentModel()
            : base()
        {
            _pa = new ObservableRegister<byte>(1);
            _sak = new ObservableRegister<byte>(1);
            _ra = new ObservableRegister<byte>(1);
            _rs = new ObservableRegister<byte>(4);
            _rk = new ObservableRegister<byte>(4);
            _or1 = new ObservableRegister<byte>(4);
            _or2 = new ObservableRegister<byte>(4);
            _acc = new ObservableRegister<byte>(4);
            _rp = new ObservableRegister<bool>(4);
            /*
            _pa = new ArrayRegister<byte>(1);
            _sak = new ArrayRegister<byte>(1);
            _ra = new ArrayRegister<byte>(1);
            _rs = new ArrayRegister<byte>(4);
            _rk = new ArrayRegister<byte>(4);
            _or1 = new ArrayRegister<byte>(4);
            _or2 = new ArrayRegister<byte>(4);
            _acc = new ArrayRegister<byte>(4);
            _rp = new ArrayRegister<bool>(4);*/

            return;
        }
    }
}
