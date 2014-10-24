using System;
using System.Collections.Generic;
using System.Linq;

namespace Apptus.ESales.EPiServer.Util
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Distinct<T, TValue>(this IEnumerable<T> source, Func<T, TValue> valueSelector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (valueSelector == null) throw new ArgumentNullException("valueSelector");
            return source.Distinct(new ValueEqualityComparer<T, TValue>(valueSelector));
        }

        public static IEnumerable<T> Distinct<T, TValue>(this IEnumerable<T> source, Func<T, TValue> valueSelector, IEqualityComparer<TValue> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (valueSelector == null) throw new ArgumentNullException("valueSelector");
            if (comparer == null) throw new ArgumentNullException("comparer");
            return source.Distinct(new ValueEqualityComparer<T, TValue>(valueSelector, comparer));
        }
    }
}