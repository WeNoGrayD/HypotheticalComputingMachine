using HypotheticalComputingMachineClassLib;
using HypotheticalComputingMachineClassLib.HypotheticalComputingMachineModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypotheticalComputingMachineApp.DataModels
{
    public class ObservableRegister<T> : ObservableArray<T>, IHCMRegister<T>
    {
        private bool _hasbeenUpdated;

        public bool HasBeenUpdated
        {
            get => _hasbeenUpdated;
            set
            {
                if (value != _hasbeenUpdated)
                {
                    _hasbeenUpdated = value;
                    OnPropertyChanged(nameof(HasBeenUpdated));
                }
            }
        }

        public ObservableRegister(int len)
            : base(len)
        {
            return;
        }

        public ObservableRegister(T[] data) : base(data)
        {
            return;
        }

        public override void Reset()
        {
            base.Reset();

            (this as IHCMRegister<T>).SoftReset();

            return;
        }

        void IHCMRegister<T>.SoftReset()
        {
            HasBeenUpdated = false;

            return;
        }

        public void LinkToProgram<THCM>(IHCMProgram<THCM> program)
            where THCM: IHypotheticalComputingMachineModel
        {
            this.PropertyChanged += (o, e) =>
            {
                if (((byte)program.State & 0b1) != 0 || e.PropertyName != IndexerName)
                    return;

                Action setUpdated = null, prepareToNextCmdExec = null, unsetUpdated = null;

                setUpdated = () =>
                {
                    program.CommandHasBeenExecuted -= setUpdated;
                    HasBeenUpdated = true;
                    //_hasbeenUpdated = true;
                    //if (((byte)program.State & 0b1) == 0b1)
                    //    OnPropertyChanged(nameof(HasBeenUpdated));
                };
                prepareToNextCmdExec = () =>
                {
                    program.PreparingToNextCommandExecuting -= prepareToNextCmdExec;
                    program.CommandHasBeenExecuted += unsetUpdated;
                };
                unsetUpdated = () =>
                {
                    program.CommandHasBeenExecuted -= unsetUpdated;
                    if (((byte)program.State & 0b10) == 0)
                    {
                        HasBeenUpdated = false;
                        //_hasbeenUpdated = false;
                        //if (((byte)program.State & 0b1) == 0b1)
                        //    OnPropertyChanged(nameof(HasBeenUpdated));
                    }
                };
                program.CommandHasBeenExecuted += setUpdated;
                program.PreparingToNextCommandExecuting += prepareToNextCmdExec;
            };

            return;
        }

        public static implicit operator T[](ObservableRegister<T> reg)
        {
            return reg.ToArray();
        }
    }
}
