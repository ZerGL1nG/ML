using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Sockets;

namespace TanksGame.Core.Constructions
{
    public class DoubleDictionary<T1, T2> : IEnumerable
    {
        private readonly Dictionary<T2, T1> _сoDictionary;
        public readonly Dictionary<T1, T2> Dictionary;
        public DoubleDictionary(Dictionary<T1, T2> dictionary)
        {
            _сoDictionary = new Dictionary<T2, T1>();
            Dictionary = dictionary;
            foreach (var (key, value) in dictionary)
                _сoDictionary.Add(value, key);
        }

        public DoubleDictionary()
        {
            _сoDictionary = new Dictionary<T2, T1>();
            Dictionary = new Dictionary<T1, T2>();
        }


        public DoubleDictionary<T1, T2> Add(T1 t1, T2 t2)
        {
            Dictionary[t1] = t2;
            _сoDictionary[t2] = t1;
           return this;
        }

        public DoubleDictionary<T1, T2> Remove(T1 t1)
        {
            if (!Dictionary.ContainsKey(t1)) return this;
            var t2 = Dictionary[t1];
            Dictionary.Remove(t1);
            _сoDictionary.Remove(t2);
            return this;
        }

        public DoubleDictionary<T1, T2> Remove(T2 t2)
        {
            var t1 = _сoDictionary[t2];
            Dictionary.Remove(t1);
            _сoDictionary.Remove(t2);
            return this;
        }

        public bool HasKey(T1 t1) => Dictionary.ContainsKey(t1);
        public bool HasValue(T2 t2) => _сoDictionary.ContainsKey(t2);

        public T2 GetValue(T1 t1) => Dictionary[t1];
        public T1 GetKey(T2 t2) => _сoDictionary[t2];

        public int Count() => Dictionary.Count;
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable) Dictionary).GetEnumerator();
        }
    }
}