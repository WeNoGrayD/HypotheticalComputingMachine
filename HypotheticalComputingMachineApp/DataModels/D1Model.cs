using HypotheticalComputingMachineClassLib.HCMModel_D1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HypotheticalComputingMachineClassLib.DICodeInterpreter;

namespace HypotheticalComputingMachineApp.DataModels
{
    internal class D1Model : HypotheticalComputingMachineD1Model
    {
        public D1Model() : base(
            (hcm) => new D1CodeSegmentModel(), 
            (hcm) => new D1DataSegmentModel(hcm), 
            (hcm) => new D1CacheSegmentModel(hcm))
        {
            return;
        }
    }
}
