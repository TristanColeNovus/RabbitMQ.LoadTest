using System;
using System.Collections.Generic;
using System.Text;

namespace Foundations.Extensions.LocalMachineCommands
{
    public interface ILocalMachineCmdFactory
    {
        ILocalMachineCmd Create(string commandString);

        ILocalMachineCmd Create(string commandName, dynamic commandArgs);
    }
}
