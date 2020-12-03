using System.Collections.Generic;
using System.Linq;
using CineMarco.EventSourcing.Csharp9.Common;
using CineMarco.EventSourcing.Csharp9.Domain;

namespace CineMarco.EventSourcing.Csharp9.Tests.Helpers
{
    public record ScreeningData(ScreeningId Id, IReadOnlyList<Seat> Seats)
    {
        public static ScreeningData ScreeningWith(int numberOfSeats) =>
            new(ScreeningId.Generate(), GenerateSeats(numberOfSeats));

        private static IReadOnlyList<Seat> GenerateSeats(int numberOfSeats) =>
            SeatNumber.Generate(numberOfSeats)
                      .Select(num => new Seat(num))
                      .ToValueList();

        public ValueList<Seat> SeatsWithNumber(params int[] numbers) =>
            Seats.Where(x => numbers.Contains(x.Number.Value))
                 .ToValueList();
    }
}