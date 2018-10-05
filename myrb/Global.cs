using System.Collections.Generic;

namespace RaptorDB
{
    public static class Global
    {
        public static bool useSortedList = false;
    }

    public class SafeSortedList<T, V> //: IKV<T, V>
    {
        private object _padlock = new object();
        SortedList<T, V> _list = new SortedList<T, V>();

        public int Count()
        {
            lock (_padlock) return _list.Count;
        }

        public void Add(T key, V val)
        {
            lock (_padlock)
            {
                if (_list.ContainsKey(key) == false)
                    _list.Add(key, val);
                else
                    _list[key] = val;
            }
        }

        public bool Remove(T key)
        {
            if (key == null)
                return true;
            lock (_padlock)
                return _list.Remove(key);
        }

        public T GetKey(int index)
        {
            lock (_padlock)
                if (index < _list.Count)
                    return _list.Keys[index];
                else
                    return default(T);
        }

        public V GetValue(int index)
        {
            lock (_padlock)
                if (index < _list.Count)
                    return _list.Values[index];
                else
                    return default(V);
        }

        public T[] Keys()
        {
            lock (_padlock)
            {
                T[] keys = new T[_list.Keys.Count];
                _list.Keys.CopyTo(keys, 0);
                return keys;
            }
        }

        public V this[T key]
        {
            get
            {
                lock (_padlock)
                    return _list[key];
            }
            set
            {
                lock (_padlock)
                    _list[key] = value;
            }
        }

        public IEnumerator<KeyValuePair<T, V>> GetEnumerator()
        {
            return ((ICollection<KeyValuePair<T, V>>)_list).GetEnumerator();
        }

        public bool TryGetValue(T key, out V value)
        {
            lock (_padlock)
                return _list.TryGetValue(key, out value);
        }

        public void Clear()
        {
            lock (_padlock)
                _list.Clear();
        }

        public V GetValue(T key)
        {
            lock (_padlock)
                return _list[key];
        }
    }
}
