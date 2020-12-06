using System.Collections.Generic;
using System.Linq;
using CineMarco.EventSourcing.Csharp9.Domain;

namespace CineMarco.EventSourcing.Csharp9.ReadSide.Models
{
    public class ScreeningInfo
    {
        public ScreeningId ScreeningId { get; }

        public IEnumerable<SeatNumber> AllSeats { get; }

        public IEnumerable<SeatNumber> AvailableSeats => _availableSeats.ToList();

        private readonly List<SeatNumber> _availableSeats;

        public ScreeningInfo(ScreeningId screeningId, IReadOnlyList<SeatNumber> allSeats)
        {
            ScreeningId     = screeningId;
            AllSeats        = allSeats.ToList();
            _availableSeats = allSeats.ToList();
        }

        public void FreeSeats(IEnumerable<SeatNumber> seatNumbers)
        {
            _availableSeats.AddRange(seatNumbers);
            _availableSeats.Sort();
        }

        public void ReserveSeats(IEnumerable<SeatNumber> seatNumbers)
        {
            foreach (var seatNumber in seatNumbers)
            {
                _availableSeats.Remove(seatNumber);
            }
        }
    }
}