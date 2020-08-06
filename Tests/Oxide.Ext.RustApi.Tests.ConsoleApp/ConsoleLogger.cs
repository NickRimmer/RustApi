using Oxide.Ext.RustApi.Interfaces;
using System;

namespace Oxide.Ext.RustApi.Tests.ConsoleApp
{
    public class ConsoleLogger<T> : ILogger<T>
    {
        /// <inheritdoc />
        public void Debug(string message)
        {
            Console.WriteLine($"[{typeof(T).Name}] {message}");
        }

        /// <inheritdoc />
        public void Error(string message)
        {
            Console.WriteLine($"[{typeof(T).Name}] {message}");
        }

        /// <inheritdoc />
        public void Error(Exception ex, string message = null)
        {
            var finalMessage = string.IsNullOrEmpty(message)
                ? ex.Message
                : message;

            Console.WriteLine($"[{typeof(T).Name}] {finalMessage}");
            Console.WriteLine(ex.Message);
            if (!string.IsNullOrEmpty(ex.StackTrace)) Console.WriteLine(ex.StackTrace);
        }

        /// <inheritdoc />
        public void Warning(string message)
        {
            Console.WriteLine($"[{typeof(T).Name}] {message}");
        }

        /// <inheritdoc />
        public void Info(string message)
        {
            Console.WriteLine($"[{typeof(T).Name}] {message}");
        }
    }
}
