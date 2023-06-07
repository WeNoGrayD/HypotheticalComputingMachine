using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace HypotheticalComputingMachineClassLib
{
    public class ObservableArray<T> : IEnumerable, IEnumerable<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        protected T[] _content;

        public int Length => _content.Length;

        public const string IndexerName = "Item[]";

        private bool _blockingIndexerChangedNotification = false;

        // Индексатор, дающий доступ к значениям словаря по ключу.

        public T this[int i]
        {
            get { return _content[i]; }
            set
            {
                if (!_content[i]?.Equals(value) ?? true)
                {
                    _content[i] = value;
                    if (!_blockingIndexerChangedNotification) OnPropertyChanged(IndexerName);
                }
            }
        }
        public ObservableArray<T> this[Range range]
        {
            get { return new ObservableArray<T>(_content[range]); }
        }

        // Конструктор.

        public ObservableArray(int size1) : base()
        {
            _content = new T[size1];
        }
        public ObservableArray(T[] data) : base()
        {
            _content = data;
        }

        public virtual void Reset()
        {
            _blockingIndexerChangedNotification = true;

            for (int i = 0; i < _content.Length; i++)
            {
                _content[i] = default(T);
            }

            _blockingIndexerChangedNotification = false;
            OnPropertyChanged(IndexerName);

            return;
        }

        public void CopyFrom(T[] data, int start)
        {
            _blockingIndexerChangedNotification = true;

            for (int i = start; i < _content.Length; i++)
            {
                this[i] = data[i - start];
            }

            _blockingIndexerChangedNotification = false;
            OnPropertyChanged(IndexerName);

            return;
        }

        public T[] ToArray()
        {
            T[] arrayCopy = new T[_content.Length];
            _content.CopyTo(arrayCopy, 0);

            return arrayCopy;
        }

        // Уведомление подписчиков на событие изменения свойства.

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _content.Length; i++)
                yield return _content[i];

            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

    public class ObservableMatrix<T> : IEnumerable, IEnumerable<T>, INotifyCollectionChanged, INotifyPropertyChanged
        where T : IEquatable<T>
    {
        public T[,] _content;

        private const string IndexerName = "Item[]";

        // Индексатор, дающий доступ к значениям словаря по ключу.

        public T this[int i, int j]
        {
            get { return _content[i, j]; }
            set
            {
                if (!_content[i, j]?.Equals(value) ?? true)
                {
                    _content[i, j] = value;
                    OnPropertyChanged(IndexerName);
                }
            }
        }

        // Конструктор.

        public ObservableMatrix(int size1, int size2) : base()
        {
            _content = new T[size1, size2];
        }

        // Уведомление подписчиков на событие изменения свойства.

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
            
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>)_content.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
    }
}
