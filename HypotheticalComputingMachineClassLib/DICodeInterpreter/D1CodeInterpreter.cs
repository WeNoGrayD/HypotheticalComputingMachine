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
    public class D1CodeInterpreter : CodeInterpreter<HypotheticalComputingMachineD1Model>
    {
        #region Fields

        private HypotheticalComputingMachineD1Model _hcm;

        #endregion

        #region Constructors

        public D1CodeInterpreter(HypotheticalComputingMachineD1Model hcm)
        {
            _hcm = hcm;
        }

        #endregion

        #region Instance methods

        protected override IHypotheticalComputingMachineCommand<HypotheticalComputingMachineD1Model>
            InterpreteCop(COP cop, byte[] copBytes)
        {
            return cop switch
            {
                COP.Halt => new HaltCommand(_hcm, copBytes),
                COP.Add => new AddCommand(_hcm, copBytes),
                COP.Sub => new SubCommand(_hcm, copBytes),
                COP.Imul => new ImulCommand(_hcm, copBytes),
                COP.Idiv => new IdivCommand(_hcm, copBytes),
                COP.And => new AndCommand(_hcm, copBytes),
                COP.Or => new OrCommand(_hcm, copBytes),
                COP.Sal => new SalCommand(_hcm, copBytes),
                COP.Sar => new SarCommand(_hcm, copBytes),
                COP.Ret => new RetCommand(_hcm, copBytes),
                COP.Call => new CallCommand(_hcm, copBytes),
                COP.Branch => new BranchCommand(_hcm, copBytes),
                COP.Jmp => new JmpCommand(_hcm, copBytes),
                COP.EndWhile => new EndWhileCommand(_hcm, copBytes),
                COP.SaveCache => new SaveCacheCommand(_hcm, copBytes),
                COP.LoadToCache => new LoadToCacheCommand(_hcm, copBytes),
                _ => throw new NotImplementedException()
            };
        }

        #endregion
    }
}
