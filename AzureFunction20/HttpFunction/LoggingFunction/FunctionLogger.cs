using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoggingFunction
{
    public  class FunctionLogger : IFunctionLogger
    {
        ILogger<FunctionLogger> _logger;
        public FunctionLogger(ILogger<FunctionLogger> logger)
        {
            _logger = logger;
        }

        public void LogCritical(string source, string content)
        {
            _logger.LogCritical($"from {source} {Environment.NewLine} {content} ");
        }

        public void LogError(string source, string content)
        {
            _logger.LogError($"from {source} {Environment.NewLine} {content} ");
        }

        public void LogInformation(string source, string content)
        {
            _logger.LogInformation($"from {source} {Environment.NewLine} {content} ");
        }
    }
}
