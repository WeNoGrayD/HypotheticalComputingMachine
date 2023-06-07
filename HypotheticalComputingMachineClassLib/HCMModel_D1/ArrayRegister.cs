using HypotheticalComputingMachineClassLib.HypotheticalComputingMachineModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace HypotheticalComputingMachineClassLib.HCMModel_D1
{
    public class ArrayRegister<T> : IHCMRegister<T>
    {
        private T[] _content;

        public T this[int i]
        {
            get => _content[i];
            set => _content[i] = value;
        }

        public int Length => _content.Length;

        public ArrayRegister(int len)
        {
            _content = new T[len];

            return;
        }

        public void Reset()
        {
            for (int i = 0; i < _content.Length; i++)
            {
                _content[i] = default(T);
            }

            return;
        }

        public void CopyFrom(T[] data, int start)
        {
            data.CopyTo(_content, start);

            return;
        }

        public T[] ToArray()
        {
            T[] registerCopy = new T[_content.Length];
            _content.CopyTo(registerCopy, 0);

            return registerCopy;
        }

        public virtual void LinkToProgram<THCM>(IHCMProgram<THCM> program)
            where THCM : IHypotheticalComputingMachineModel
        { return; }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _content.Length; i++) yield return _content[i];

            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
