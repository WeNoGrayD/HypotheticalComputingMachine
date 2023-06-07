using HypotheticalComputingMachineClassLib.HypotheticalComputingMachineModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypotheticalComputingMachineClassLib.HCMModel_D1
{
    public class CacheSegmentBase : DataSegmentBase
    {
        public CacheSegmentBase(
            IHypotheticalComputingMachineModel hcm, 
            int memoryCellCount, 
            int memoryCellBytesCount,
            Func<int, int, IHCMRegister<byte>> registerFactory)
            : base(hcm, memoryCellCount, memoryCellBytesCount, registerFactory)
        {
            return;
        }

        public override void WriteToMemCell(int address, byte[] data, bool makeSideEffects = true)
        {
            this[address] = data;

            return;
        }

        public override byte[] ReadFromMemCell(int address, bool makeSideEffects = true)
        {
            return this[address];
        }
    }

    public class D1CacheSegment : CacheSegmentBase
    {
        public D1CacheSegment(IHypotheticalComputingMachineModel hcm)
            : base(hcm, 16, 4, (_, memoryCellBytesCount) => new ArrayRegister<byte>(memoryCellBytesCount))
        {
            return;
        }
    }
}
