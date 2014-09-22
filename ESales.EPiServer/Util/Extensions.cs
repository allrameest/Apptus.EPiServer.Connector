using System;
using System.Collections.Generic;
using System.Linq;

namespace Apptus.ESales.EPiServer.Util
{
    public static class Extensions
    {
        public static void Add<TKey, TValue>( this IDictionary<TKey, List<TValue>> dictionary, TKey key, TValue value )
        {
            List<TValue> values;
            if ( !dictionary.TryGetValue( key, out values ) )
            {
                values = new List<TValue>();
                dictionary.Add( key, values );
            }
            values.Add( value );
        }

        public static void Add<TKey, TValue>( this IDictionary<TKey, HashSet<TValue>> dictionary, TKey key, TValue value )
        {
            HashSet<TValue> values;
            if ( !dictionary.TryGetValue( key, out values ) )
            {
                values = new HashSet<TValue>();
                dictionary.Add( key, values );
            }
            values.Add( value );
        }

        public static void AddOnce<TKey, TValue>( this IDictionary<TKey, TValue> dictionary, TKey key, TValue value )
        {
            if ( !dictionary.ContainsKey( key ) )
            {
                dictionary.Add( key, value );
            }
        }

        public static void Remove<TKey, TValue>( this IDictionary<TKey, HashSet<TValue>> dictionary, TKey key, TValue value )
        {
            HashSet<TValue> values;
            if ( dictionary.TryGetValue( key, out values ) )
            {
                values.Remove( value );
            }
        }

        public static IEnumerable<T> EmptyIfNull<T>( this IEnumerable<T> source )
        {
            if ( source != null )
            {
                return source.Select( s => s );
            }
            return Enumerable.Empty<T>();
        }

        public static IEnumerable<string> Split( this string source, string separator, StringSplitOptions option )
        {
            if ( !string.IsNullOrEmpty( source ) )
            {
                return source.Split( new[] {separator}, option ).Select( s => s );
            }
            return Enumerable.Empty<string>();
        }

        public static IEnumerable<T> Append<T>( this IEnumerable<T> source, T item, bool appendEmptyString = false )
        {
            var stringItem = item as string;
            if ( !appendEmptyString && stringItem != null && stringItem.Trim() == "" )
            {
                return source ?? Enumerable.Empty<T>();
            }
            var itemList = item == null ? Enumerable.Empty<T>() : Enumerable.Repeat( item, 1 );
            return source == null ? itemList : source.Concat( itemList );
        }
    }
}
