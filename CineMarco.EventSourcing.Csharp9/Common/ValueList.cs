using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CineMarco.EventSourcing.Csharp9.Common
{
    /// <summary>
    /// Read only list comparable by value
    /// </summary>
    public sealed class ValueList<T> : IReadOnlyList<T>, IEquatable<ValueList<T>>
    {
        private readonly IReadOnlyList<T> _items;

        public ValueList(IEnumerable<T> items)
        {
            _items = items.ToList();
        }

        public int Count => _items.Count;

        public T this[int index] => _items[index];

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _items).GetEnumerator();

        public bool Equals(ValueList<T>? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _items.SequenceEqual(other._items);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ValueList<T>) obj);
        }

        public override int GetHashCode() => _items.GetHashCode();

        public static bool operator ==(ValueList<T>? left, ValueList<T>? right) => Equals(left, right);

        public static bool operator !=(ValueList<T>? left, ValueList<T>? right) => !Equals(left, right);

        public override string ToString() => $"[{string.Join(", ", _items.Select(x => $"{x}"))}]";
    }

    public static class ValueListExtensions
    {
        public static ValueList<T> ToValueList<T>(this IEnumerable<T> source) => new(source);
    }
}