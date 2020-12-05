using System;

namespace CineMarco.EventSourcing.Csharp9.Tests.Utils.Fixtures
{
    public static class Planned
    {
        public static DateTimeOffset Later(int minutes) =>
            DateTimeOffset.UtcNow.AddMinutes(minutes);

        public static DateTimeOffset Tomorrow =>
            DateTimeOffset.UtcNow.AddDays(1);
    }
}