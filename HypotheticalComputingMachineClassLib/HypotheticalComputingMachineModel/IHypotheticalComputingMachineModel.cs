using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HypotheticalComputingMachineClassLib.HCMModel_D1;

namespace HypotheticalComputingMachineClassLib.HypotheticalComputingMachineModel
{
    public interface IHypotheticalComputingMachineModel
    {
        IHCMCodeSegment CS { get; }

        IHCMDataSegment DS { get; }

        IHCMProgram<IHypotheticalComputingMachineModel> Program { get; }

        void Reset();

        void SoftReset();

        void WriteData(byte[] data, int address);

        void LoadFromFile(Stream config);

        void LoadFromFile(string configPath) => LoadFromFile(new FileStream(configPath, FileMode.Open));

        void SaveToFile(Stream config);

        void SaveToFile(string configPath) => SaveToFile(new FileStream(configPath, FileMode.OpenOrCreate));
    }

    public interface IHypotheticalComputingMachineD1Model : IHypotheticalComputingMachineModel
    {
        IHCMDataSegment Cache { get; }
    }
}
