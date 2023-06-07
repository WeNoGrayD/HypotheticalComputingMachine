using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypotheticalComputingMachineTests
{
    public class HCMTests
    {
        protected HypotheticalComputingMachineD1Model _hcm;

        protected IHCMProgram<IHypotheticalComputingMachineModel> _program;

        protected static byte _defaultDsStartAddress = 0;
        protected static byte _defaultCsStartAddress = 40;

        protected AddressSpace _dsAddress = new AddressSpace(_defaultDsStartAddress);
        protected AddressSpace _csAddress = new AddressSpace(_defaultCsStartAddress);

        public HCMTests()
        {
            _hcm = new HypotheticalComputingMachineD1Model();

            return;
        }

        protected void RestartHCM()
        {
            (_hcm as IHypotheticalComputingMachineD1Model).Reset();
            _hcm.CS.StartCSFrom(_defaultCsStartAddress);
            RestartAddressSpaces();
            _program = _hcm.Program;

            return;
        }

        protected void RestartAddressSpaces()
        {
            _dsAddress = new AddressSpace(_defaultDsStartAddress);
            _csAddress = new AddressSpace(_defaultCsStartAddress);

            return;
        }

        protected void CompareDataInDS(int address, byte[] data)
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_hcm.DS.ReadFromMemCell(address, false), data));

            return;
        }

        protected void CompareDataInCache(int address, byte[] data)
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_hcm.Cache.ReadFromMemCell(address, false), data));

            return;
        }

        protected void CompareRP(bool[] rp)
        {
            Assert.IsTrue(Enumerable.SequenceEqual(_hcm.CS.RP, rp));

            return;
        }

        #region Inner classes

        protected class AddressSpace
        {
            private IEnumerator<int> _addressEnum;

            private int _startAddress;

            private int _step;

            public AddressSpace(int startAddress)
            {
                _startAddress = startAddress;
                Restart();

                return;
            }

            public int Prev(int n)
            {
                _step = -1;
                MoveNextAndCatchError();

                return _addressEnum.Current;
            }

            public int PrevOverNTimes(int n)
            {
                _step = -n;
                MoveNextAndCatchError();

                return _addressEnum.Current;
            }

            public int Next()
            {
                _step = 1;
                MoveNextAndCatchError();

                return _addressEnum.Current;
            }

            public int NextOverNTimes(int n)
            {
                _step = n;
                MoveNextAndCatchError();

                return _addressEnum.Current;
            }

            public void Restart()
            {
                _addressEnum = Crawler().GetEnumerator();
            }

            public void MoveNextAndCatchError()
            {
                if (!_addressEnum.MoveNext())
                    throw new Exception("Need more memory");
            }

            private IEnumerable<int> Crawler()
            {
                for (int address = _startAddress; address >= 0 && address <= 256; address += _step)
                {
                    yield return address;
                }

                yield break;
            }
        }

        #endregion
    }
}
