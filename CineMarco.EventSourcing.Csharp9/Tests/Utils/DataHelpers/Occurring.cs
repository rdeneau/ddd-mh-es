using System;
using CineMarco.EventSourcing.Csharp9.Common;

namespace CineMarco.EventSourcing.Csharp9.Tests.Utils.DataHelpers
{
    public static class Occurring
    {
        public static DateTimeOffset Later(int minutes) =>
            ClockUtc.Now.Add(new TimeSpan(hours: 0, minutes, seconds: 1)); // 1 second margin to compensate future "Now" during test

        public static DateTimeOffset Sooner(int minutesAgo) =>
            ClockUtc.Now.AddMinutes(-minutesAgo);

        public static DateTimeOffset Tomorrow =>
            ClockUtc.Now.AddDays(1);
    }
}