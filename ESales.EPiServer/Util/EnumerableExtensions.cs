using System;
using System.Collections.Generic;

namespace Apptus.ESales.EPiServer.Util
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IReadOnlyCollection<T>> ToChunks<T>(this IEnumerable<T> source, int chunkSize)
        {
            if (source == null) throw new ArgumentNullException("source");

            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yield return ChunkElements(enumerator, chunkSize - 1);
                }
            }
        }

        private static IReadOnlyCollection<T> ChunkElements<T>(IEnumerator<T> source, int batchSize)
        {
            var list = new List<T>(batchSize)
                {
                    source.Current
                };

            for (var i = 0; i < batchSize && source.MoveNext(); i++)
            {
                list.Add(source.Current);
            }

            return list;
        }
    }
}