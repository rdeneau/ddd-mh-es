using System;

namespace CineMarco.EventSourcing.Csharp9.Common
{
    public static class ClockUtc
    {
        public static DateTimeOffset Now =>
            DateTimeOffset.UtcNow
                          .With(d => d.AddMilliseconds(-d.Millisecond));
    }
}