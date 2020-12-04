using System;

namespace CineMarco.EventSourcing.Csharp9.Common
{
    public static class ObjectExtensions
    {
        public static T With<T>(this T source, Action<T>? action)
        {
            action?.Invoke(source);
            return source;
        }

        public static T With<T>(this T source, Func<T, T>? action) =>
            action != null ? action(source) : source;
    }
}