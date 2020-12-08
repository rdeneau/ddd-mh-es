using CineMarco.EventSourcing.Csharp9.Domain;

namespace CineMarco.EventSourcing.Csharp9.Tests.Utils.DataHelpers
{
    public static class Screenings
    {
        public static ScreeningId Screening1 { get; } = ScreeningId.Generate();

        public static ScreeningId Screening2 { get; } = ScreeningId.Generate();
    }
}