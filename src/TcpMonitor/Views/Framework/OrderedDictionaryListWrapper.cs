using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace TcpMonitor.Views.Framework
{
    public class OrderedDictionaryListWrapper : IList
    {
        private const string ReadonlyMessage = "The OrderedDictionaryListWrapper is a ReadOnly Ilist";

        private readonly OrderedDictionary _backingDictionary;

        public OrderedDictionaryListWrapper(OrderedDictionary backingDictionary)
        {
            _backingDictionary = backingDictionary;
        }

        public IEnumerator GetEnumerator() => _backingDictionary.GetEnumerator();

        public void CopyTo(Array array, int index) => _backingDictionary.Values.CopyTo(array, index);

        public int Count => _backingDictionary.Count;
        public object SyncRoot => _backingDictionary.Values.SyncRoot;
        public bool IsSynchronized => _backingDictionary.Values.IsSynchronized;

        public bool Contains(object value)
        {
            return _backingDictionary.Contains(value ?? new object());
        }

        public int IndexOf(object value)
        {
            var index = 0;
            while (index <= Count)
            {
                var item = this[index];
                if (item.Equals(value)) return index;
                index++;
            }

            return -1;
        }

        public object this[int index]
        {
            get => _backingDictionary[index];
            set => throw new InvalidOperationException(ReadonlyMessage);
        }

        public IEnumerable<T> Cast<T>()
        {
            return _backingDictionary.Values.Cast<T>();
        }

        public bool IsReadOnly => true;

        public bool IsFixedSize => false;

        public int Add(object value)
        {
            throw new InvalidOperationException(ReadonlyMessage);
        }

        public void Insert(int index, object value)
        {
            throw new InvalidOperationException(ReadonlyMessage);
        }

        public void Remove(object value)
        {
            throw new InvalidOperationException(ReadonlyMessage);
        }

        public void RemoveAt(int index)
        {
            throw new InvalidOperationException(ReadonlyMessage);
        }

        public void Clear()
        {
            throw new InvalidOperationException(ReadonlyMessage);
        }
    }
}
