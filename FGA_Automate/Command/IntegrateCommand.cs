using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine.Utility;

namespace FGA.Automate.Command
{
    interface IntegrateCommand
    {
        void Execute(Arguments CommandLine);
        string usage();
    }
}
