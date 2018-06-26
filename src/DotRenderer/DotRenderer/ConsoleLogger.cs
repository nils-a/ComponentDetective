using ComponentDetective.Contracts;
using System;

namespace ComponentDetective.DotRender
{
    internal class ConsoleLogger : ILogger
    {
        private readonly bool verbose;

        internal ConsoleLogger(bool verbose)
        {
            this.verbose = verbose;
        }

        public void Error(string message)
        {
            Console.Error.WriteLine($"ERROR: {message}");
        }

        public void Error(string message, Exception exception)
        {
            Console.Error.WriteLine($"ERROR: {message}");
            Console.Error.WriteLine($"       {exception.GetType().Name}: {exception.Message}");
        }

        public void Fatal(string message, Exception exception)
        {
            Console.Error.WriteLine($"FATAL: {message}");
            Console.Error.WriteLine($"       {exception.GetType().Name}: {exception.Message}");
            Console.Error.WriteLine($"{exception.StackTrace}");
        }

        public void Verbose(string message)
        {
            if (!verbose)
            {
                return;
            }
            Console.Out.WriteLine(message);
        }
    }
}
