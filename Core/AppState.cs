using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultimedijskiPredvajalnik.Core
{
    public enum AppState
    {
        Running,
        ItemSelected,
    }

    public enum VoiceCommand
    {
        Play,
        Stop,
        Next,
        Previous,
        Select,
        Remove,
        Exit,
        Add,
        Edit,
        Unknown
    }
}
