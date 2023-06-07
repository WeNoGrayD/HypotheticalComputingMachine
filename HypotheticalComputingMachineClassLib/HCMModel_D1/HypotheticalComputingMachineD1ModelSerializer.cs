using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HypotheticalComputingMachineClassLib.HCMModel_D1
{
    internal class HypotheticalComputingMachineD1ModelSerializer
    {
        public void SaveToFile(HypotheticalComputingMachineD1Model hcm, Stream configStream)
        {
            using (StreamWriter swConfig = new StreamWriter(configStream))
            {
                swConfig.WriteLine($"[ PA = {BinaryInOutConverter.BytesToInt32(hcm.CS.PA)} ]");
                swConfig.WriteLine("{ DS:");

                IHCMDataSegment ds = hcm.DS, cache = hcm.Cache;

                for (int i = 0; i < ds.Length; i++)
                {
                    swConfig.WriteLine($"{i.ToString().PadLeft(3)}| " + string.Join(' ', ds.ReadFromMemCell(i, false).Select(BinaryInOutConverter.ByteToBinaryStr)));
                }
                swConfig.WriteLine(" }");
                swConfig.WriteLine("{ Cache:");
                for (int i = 1; i < cache.Length; i++)
                {
                    swConfig.WriteLine($"{i.ToString().PadLeft(2)}| " + string.Join(' ', cache.ReadFromMemCell(i, false).Select(BinaryInOutConverter.ByteToBinaryStr)));
                }
                swConfig.Write(" }");
            }

            return;
        }
    }
}
