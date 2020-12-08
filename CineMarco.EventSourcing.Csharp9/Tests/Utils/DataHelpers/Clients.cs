using CineMarco.EventSourcing.Csharp9.Domain;

namespace CineMarco.EventSourcing.Csharp9.Tests.Utils.DataHelpers
{
    public static class Clients
    {
        public static ClientId Client1 { get; } = ClientId.Generate();

        public static ClientId Client2 { get; } = ClientId.Generate();
    }
}