using System;
using CineMarco.EventSourcing.Csharp9.Common;

namespace CineMarco.EventSourcing.Csharp9.Tests.Utils.DataHelpers
{
    public static class Occurring
    {
        public static DateTimeOffset Later(int minutes) =>
            ClockUtc.Now.AddMinutes(minutes);

        public static DateTimeOffset Sooner(int minutes) =>
            ClockUtc.Now.AddMinutes(-minutes);

        public static DateTimeOffset Tomorrow =>
            ClockUtc.Now.AddDays(1);
    }
}