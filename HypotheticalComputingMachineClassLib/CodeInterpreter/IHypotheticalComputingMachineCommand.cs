using HypotheticalComputingMachineClassLib.HCMModel_D1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypotheticalComputingMachineClassLib.CodeInterpreter
{
    public interface IHypotheticalComputingMachineCommand<THypotheticalComputingMachineModel>
    {
        public THypotheticalComputingMachineModel HCM { get; }

        public abstract void Execute();
    }
}
