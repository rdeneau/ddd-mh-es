using System.Collections.Generic;
using System.Linq;

namespace CineMarco.EventSourcing.Csharp9.Domain
{
    public sealed record SeatNumber
    {
        public static IEnumerable<SeatNumber> Generate(int count) =>
            Enumerable.Range(1, count).Select(value => new SeatNumber { Value = value });

        public int Value { get; init; }

        private SeatNumber() { }
    }
}