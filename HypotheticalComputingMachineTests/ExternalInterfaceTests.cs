using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using HypotheticalComputingMachineClassLib;
using System.Globalization;

namespace HypotheticalComputingMachineTests
{
    [TestClass]
    public class ExternalInterfaceTests : HCMTests
    {
        private static string _configsPath;

        static ExternalInterfaceTests()
        {
            _configsPath = Directory
                .GetParent(Environment.CurrentDirectory)
                .Parent
                .Parent.FullName;
                //.GetDirectories()
                //.First(dir => dir.Name == "configs").FullName;
        }

        [TestInitialize]
        public void Init()
        {
            RestartHCM();

            return;
        }

        private void CompareDataSegments(IHCMDataSegment ds1, IHCMDataSegment ds2)
        {
            Assert.IsTrue(ds1.Length == ds2.Length);
            for (int i = 0; i < ds1.Length; i++)
                Assert.IsTrue(Enumerable.SequenceEqual(ds1.ReadFromMemCell(i, false), ds2.ReadFromMemCell(i, false)));

            return;
        }

        [TestMethod]
        public void SerializeHCM()
        {
            _dsAddress.Next();
            _dsAddress.NextOverNTimes(9);
            for (byte i = 1; i <= 10; i++)
                _hcm.WriteData(new byte[4] { 0b0, 0b0, 0b0, i }, _dsAddress.Next());
            _dsAddress.NextOverNTimes(10);
            _hcm.WriteData(new byte[4] { 0b00001001, 0b00000001, 0b0, 0b00000001 }, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b0, 0b0, 0b0, 0b00001111 }, _dsAddress.Next());

            _hcm.WriteData(new byte[4] { 0b11110001, 0b00011110, 0b0, 0b0 }, _csAddress.Next()); // команда load to cache
            _hcm.WriteData(new byte[4] { 0b01110001, 0b00001001, 0b00000100, 0b00010011 }, _csAddress.Next()); // команда sal
            _hcm.WriteData(new byte[4] { 0b00010001, 0b00010011, 0b00011111, 0b00010011 }, _csAddress.Next()); // команда add
            _hcm.WriteData(new byte[4] { 0b11010001, 0b00011110, 0b00101001, 0b00101100 }, _csAddress.Next()); // команда end while
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();

            using (FileStream config = new FileStream(_configsPath + @"\test.txt", FileMode.OpenOrCreate))
            {
                _hcm.SaveToFile(config);
            }
        }

        [TestMethod]
        public void SerializeAndDeserializeHCM()
        {
            _dsAddress.Next();
            _dsAddress.NextOverNTimes(9);
            for (byte i = 1; i <= 10; i++)
                _hcm.WriteData(new byte[4] { 0b0, 0b0, 0b0, i }, _dsAddress.Next());
            _dsAddress.NextOverNTimes(10);
            _hcm.WriteData(new byte[4] { 0b00001001, 0b00000001, 0b0, 0b00000001 }, _dsAddress.Next());
            _hcm.WriteData(new byte[4] { 0b0, 0b0, 0b0, 0b00001111 }, _dsAddress.Next());

            _hcm.WriteData(new byte[4] { 0b11110001, 0b00011110, 0b0, 0b0 }, _csAddress.Next()); // команда load to cache
            _hcm.WriteData(new byte[4] { 0b01110001, 0b00001001, 0b00000100, 0b00010011 }, _csAddress.Next()); // команда sal
            _hcm.WriteData(new byte[4] { 0b00010001, 0b00010011, 0b00011111, 0b00010011 }, _csAddress.Next()); // команда add
            _hcm.WriteData(new byte[4] { 0b11010001, 0b00011110, 0b00101001, 0b00101100 }, _csAddress.Next()); // команда end while
            _hcm.WriteData(new byte[4], _csAddress.Next()); // команда halt (останов)
            _program.RunTilEnd();

            using (FileStream config = new FileStream(_configsPath + @"\test2.txt", FileMode.OpenOrCreate))
            {
                _hcm.SaveToFile(config);
            }

            HypotheticalComputingMachineD1Model hcm2 = new HypotheticalComputingMachineD1Model();

            using (FileStream config = new FileStream(_configsPath + @"\test2.txt", FileMode.Open))
            {
                hcm2.LoadFromFile(config);
            }

            Assert.IsTrue(Enumerable.SequenceEqual(_hcm.CS.PA, hcm2.CS.PA));
            CompareDataSegments(_hcm.DS, hcm2.DS);
            CompareDataSegments(_hcm.Cache, hcm2.Cache);
        }
    }
}
