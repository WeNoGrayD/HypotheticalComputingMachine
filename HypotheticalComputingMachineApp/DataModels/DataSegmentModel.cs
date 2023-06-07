using HypotheticalComputingMachineClassLib;
using HypotheticalComputingMachineClassLib.HCMModel_D1;
using HypotheticalComputingMachineClassLib.HypotheticalComputingMachineModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace HypotheticalComputingMachineApp.DataModels
{
    public class IndexedMemoryCell : INotifyPropertyChanged
    {
        private IHCMRegister<byte> _reg;

        public int Index { get; private set; }

        public IHCMRegister<byte> Register { get => _reg; }

        public byte[] Reg
        {
            get => _reg.ToArray();
            set
            {
                if (!Enumerable.SequenceEqual(value, _reg))
                {
                    value.ShiftIn(_reg);
                }
            }
        }

        public IndexedMemoryCell(int index, IHCMRegister<byte> reg)
        {
            Index = index;
            ObservableRegister<byte> observableReg = reg as ObservableRegister<byte>;
            observableReg.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == ObservableRegister<byte>.IndexerName)
                {
                    this.OnPropertyChanged(nameof(Reg));
                }
            };
            _reg = observableReg;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class DataSegmentModel : DataSegmentBase, INotifyPropertyChanged
    {
        private int _prevVisibleMemoryStart;

        private int _visibleMemoryStart = 0;

        public int VisibleMemoryStart
        {
            get => _visibleMemoryStart;
            set 
            {
                if (value != _visibleMemoryStart)
                {
                    _prevVisibleMemoryStart = _visibleMemoryStart;
                    _visibleMemoryStart = value;
                    OnPropertyChanged(nameof(VisibleMemoryStart));
                }
            }
        }

        private int _prevVisibleMemoryEnd;

        private int _visibleMemoryEnd;

        public int VisibleMemoryEnd
        {
            get => _visibleMemoryEnd;
            set
            {
                if (value != _visibleMemoryEnd)
                {
                    _prevVisibleMemoryEnd = _visibleMemoryEnd;
                    _visibleMemoryEnd = value;
                    OnPropertyChanged(nameof(VisibleMemoryEnd));
                }
            }
        }

        public ObservableCollection<IndexedMemoryCell> Memory { get; set; } = new ObservableCollection<IndexedMemoryCell>();

        public DataSegmentModel(IHypotheticalComputingMachineModel hcm, int memoryCellCount, int memoryCellBytesCount)
            : base(
                  hcm, 
                  memoryCellCount, 
                  memoryCellBytesCount,
                  (index, memCellBytesCount) => new ObservableRegister<byte>(memCellBytesCount))
        {
            _visibleMemoryEnd = memoryCellCount - 1;
            for (int i = 0; i < memoryCellCount; i++)
                Memory.Add(new IndexedMemoryCell(i, _memory[i]));

            PropertyChanged += IndexChanged;

            return;
        }

        private void IndexChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VisibleMemoryStart))
            {
                if (_visibleMemoryStart > _prevVisibleMemoryStart)
                {
                    for (int i = 0; i < _visibleMemoryStart - _prevVisibleMemoryStart; i++)
                        Memory.RemoveAt(0);
                }
                else
                {
                    int j = 0;
                    for (int i = _visibleMemoryStart; i < _prevVisibleMemoryStart; i++, j++)
                        Memory.Insert(j, new IndexedMemoryCell(i, _memory[i]));
                }
            }
            else if (e.PropertyName == nameof(VisibleMemoryEnd))
            {
                if (_visibleMemoryEnd < _prevVisibleMemoryEnd)
                {
                    int nextAfterCurrentEnd = (_visibleMemoryEnd + 1) - _visibleMemoryStart;
                    for (int i = nextAfterCurrentEnd; i <= (_prevVisibleMemoryEnd - _visibleMemoryStart); i++)
                        Memory.RemoveAt(nextAfterCurrentEnd);
                }
                else
                {
                    for (int i = _prevVisibleMemoryEnd + 1; i <= _visibleMemoryEnd; i++)
                        Memory.Add(new IndexedMemoryCell(i, _memory[i]));
                }
            }

            return;
        }

        public override void LinkToProgram<THCM>(IHCMProgram<THCM> program)
        {
            foreach (var reg in _memory) reg.LinkToProgram(program);
            return;
        }

        // Уведомление подписчиков на событие изменения свойства.

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class D1DataSegmentModel : DataSegmentModel
    {
        public D1DataSegmentModel(IHypotheticalComputingMachineModel hcm)
            : base(hcm, 256, 4)
        {
            return;
        }
    }

    public class CacheSegmentModel : CacheSegmentBase
    {
        public IndexedMemoryCell[] Memory { get; set; }

        public CacheSegmentModel(IHypotheticalComputingMachineModel hcm, int memoryCellCount, int memoryCellBytesCount)
            : base(
                  hcm, 
                  memoryCellCount, 
                  memoryCellBytesCount, 
                  (_, memoryCellBytesCount) => new ObservableRegister<byte>(memoryCellBytesCount))
        {
            Memory = new IndexedMemoryCell[memoryCellCount];
            for (int i = 0; i < memoryCellCount; i++)
                Memory[i] = new IndexedMemoryCell(i, _memory[i]);

            return;
        }

        public override void LinkToProgram<THCM>(IHCMProgram<THCM> program)
        {
            foreach (var reg in _memory) reg.LinkToProgram(program);
            return;
        }
    }

    public class D1CacheSegmentModel : CacheSegmentModel
    {
        public D1CacheSegmentModel(IHypotheticalComputingMachineModel hcm)
            : base(hcm, 16, 4)
        {
            return;
        }
    }
}
