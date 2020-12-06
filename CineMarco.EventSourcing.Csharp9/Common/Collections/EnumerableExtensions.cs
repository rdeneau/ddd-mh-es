using System;
using System.Collections.Generic;

namespace CineMarco.EventSourcing.Csharp9.Common.Collections
{
    public static class EnumerableExtensions
    {
        public static ReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> source) => new(source);

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> @do)
        {
            foreach (var item in source)
            {
                @do(item);
            }
        }

    }
}