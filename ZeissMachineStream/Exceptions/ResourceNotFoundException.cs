using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZeissMachineStream.Exceptions
{
    public class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException(String message) : base(message)
        {

        }
    }
}
