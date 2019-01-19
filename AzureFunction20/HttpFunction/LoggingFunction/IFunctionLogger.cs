using System;
using System.Collections.Generic;
using System.Text;

namespace LoggingFunction
{
    public interface IFunctionLogger
    {
        void LogInformation(string source, string content);
        void LogError(string source, string content);
        void LogCritical(string source, string content);


    }
}
