using System;
using System.Collections.Generic;

namespace Apptus.ESales.EPiServer.Util
{
    public class ValueEqualityComparer<T, TValue> : IEqualityComparer<T>
    {
        private readonly Func<T, TValue> _valueSelector;
        private readonly IEqualityComparer<TValue> _comparer;

        public ValueEqualityComparer(Func<T, TValue> valueSelector, IEqualityComparer<TValue> comparer = null)
        {
            if (valueSelector == null) throw new ArgumentNullException("valueSelector");
            _valueSelector = valueSelector;
            _comparer = comparer ?? EqualityComparer<TValue>.Default;
        }

        public bool Equals(T x, T y)
        {
            return _comparer.Equals(_valueSelector(x), _valueSelector(y));
        }

        public int GetHashCode(T obj)
        {
            return _comparer.GetHashCode(_valueSelector(obj));
        }
    }
}