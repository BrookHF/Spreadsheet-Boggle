using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoggleClient
{
    interface IBoggle
    {
        event Action LoadPlayFormEvent;
        event Action LoginEvent;
        event Action CancelEvent;
        event Action EnterEvent;
        event Action LeaveEvent;
        event Action HelpGameRulesEvent;
        event Action HelpHowToPlayEvent;
    }
}
