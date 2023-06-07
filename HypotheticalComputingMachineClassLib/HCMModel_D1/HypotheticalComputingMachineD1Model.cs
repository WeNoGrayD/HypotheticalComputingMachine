using HypotheticalComputingMachineClassLib.HypotheticalComputingMachineModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HypotheticalComputingMachineClassLib.DICodeInterpreter;

namespace HypotheticalComputingMachineClassLib.HCMModel_D1
{
    public class HypotheticalComputingMachineD1Model : IHypotheticalComputingMachineD1Model
    {
        private Lazy<HypotheticalComputingMachineD1ModelSerializer> _serializer = 
            new Lazy<HypotheticalComputingMachineD1ModelSerializer>();
        private Lazy<HypotheticalComputingMachineD1ModelDeserializer> _deserializer = 
            new Lazy<HypotheticalComputingMachineD1ModelDeserializer>();

        internal HypotheticalComputingMachineD1ModelSerializer Serializer => _serializer.Value;

        internal HypotheticalComputingMachineD1ModelDeserializer Deserializer => _deserializer.Value;

        public IHCMCodeSegment CS { get; private set; }

        public IHCMDataSegment DS { get; private set; }

        public IHCMDataSegment Cache { get; private set; }

        public IHCMProgram<IHypotheticalComputingMachineModel> Program { get; private set; }

        protected HypotheticalComputingMachineD1Model(
            Func<HypotheticalComputingMachineD1Model, IHCMCodeSegment> csFactory,
            Func<HypotheticalComputingMachineD1Model, IHCMDataSegment> dsFactory,
            Func<HypotheticalComputingMachineD1Model, IHCMDataSegment> cacheFactory)
        {
            DS = dsFactory(this);
            CS = csFactory(this);
            Cache = cacheFactory(this);
            Program = new HCMProgram<HypotheticalComputingMachineD1Model>(this, (hcm) => new D1CodeInterpreter(hcm));

            LinkToProgram();
        }

        public HypotheticalComputingMachineD1Model()
        {
            DS = new D1DataSegment(this);
            CS = new D1CodeSegment();
            Cache = new D1CacheSegment(this);
            Program = new HCMProgram<HypotheticalComputingMachineD1Model>(this, (hcm) => new D1CodeInterpreter(hcm));

            LinkToProgram();
        }

        public void Reset()
        {
            Program.Reset();
            CS.Reset();
            DS.Reset();
            Cache.Reset();

            return;
        }

        public void SoftReset()
        {
            Program.Reset();
            CS.SoftReset();
            DS.SoftReset();
            Cache.SoftReset();

            return;
        }

        protected void LinkToProgram()
        {
            CS.LinkToProgram(Program);
            DS.LinkToProgram(Program);
            Cache.LinkToProgram(Program);

            return;
        }

        public void WriteData(byte[] data, int address)
        {
            DS.WriteToMemCell(address, data);

            return;
        }

        public void LoadFromFile(Stream config)
        {
            Deserializer.LoadFromFile(this, config);

            return;
        }

        public void SaveToFile(Stream config)
        {
            (this as IHypotheticalComputingMachineD1Model).Reset();
            Serializer.SaveToFile(this, config);

            return;
        }
    }
}
