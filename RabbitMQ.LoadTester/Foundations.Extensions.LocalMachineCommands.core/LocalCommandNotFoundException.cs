using System;
using System.Collections.Generic;
using System.Text;

namespace Foundations.Extensions.LocalMachineCommands
{
    public class LocalCommandNotFoundException : Exception
    {
        public LocalCommandNotFoundException(string message) : base(message)
        {
        }
    }
}
