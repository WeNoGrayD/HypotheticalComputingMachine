using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypotheticalComputingMachineClassLib.HypotheticalComputingMachineModel
{
    public interface IHCMRegister<T> : IEnumerable<T>
    {
        T this[int i] { get; set; }

        int Length { get; }

        void Reset();

        void SoftReset() { return; }

        void CopyFrom(T[] data, int start);

        T[] ToArray();

        void LinkToProgram<THCM>(IHCMProgram<THCM> program)
            where THCM : IHypotheticalComputingMachineModel;
    }

    public static class HCMRegisterHelper
    {
        public static void ShiftIn<T>(this T[] data, IHCMRegister<T> reg)
        {
            int j = reg.Length - 1;
            for (int i = data.Length - 1; j >= 0 && i >= 0; i--, j--)
            {
                reg[j] = data[i];
            }

            return;
        }

        public static byte[] Increment(this byte[] reg)
        {
            int inc = BinaryInOutConverter.BytesToInt32(reg) + 1;
            return BinaryInOutConverter.Int32ToBytes(inc);
        }
    }
}
