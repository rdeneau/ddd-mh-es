using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CineMarco.EventSourcing.Csharp9.Common.Collections
{
    /// <summary>
    /// Read only list comparable by value
    /// </summary>
    public sealed class ReadOnlyList<T> : IReadOnlyList<T>, IEquatable<ReadOnlyList<T>>
    {
        private readonly List<T> _items = new();

        public ReadOnlyList() {}
        public ReadOnlyList(IEnumerable<T> items) => _items.AddRange(items);

        public int Count => _items.Count;

        public T this[int index] => _items[index];

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _items).GetEnumerator();

        public bool Equals(ReadOnlyList<T>? other)
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
            return Equals((ReadOnlyList<T>) obj);
        }

        public override int GetHashCode() => _items.GetHashCode();

        public static bool operator ==(ReadOnlyList<T>? left, ReadOnlyList<T>? right) => Equals(left, right);

        public static bool operator !=(ReadOnlyList<T>? left, ReadOnlyList<T>? right) => !Equals(left, right);

        public override string ToString() => $"[{string.Join(", ", _items.Select(x => $"{x}"))}]";

        public ReadOnlyList<T> WithEach(Action<T> changeItem)
        {
            _items.ForEach(changeItem);
            return this;
        }

        public static ReadOnlyList<T> operator +(ReadOnlyList<T> left, ReadOnlyList<T> right) =>
            new(left.Concat(right));
    }
}