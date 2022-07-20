using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZeissMachineStream.Exceptions
{
    public class RequestValidationException : Exception
    {
        public RequestValidationException(String message) : base(message)
        {

        }
    }
}
