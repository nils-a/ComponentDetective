using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentDetective.Contracts
{
    public interface ILogger
    {
        void Verbose(string message);
        void Error(string message);
        void Error(string message, Exception exception);
        void Fatal(string message, Exception exception);
    }
}
