using Protocols.Code;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protocols
{
    public class LogicException : Exception
    {
        public Result Result { get; set; }

        public LogicException(Result result)
        {
            Result = result;
        }

        public LogicException()
        {
        }

        public LogicException(string message) : base(message)
        {
        }

        public LogicException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
