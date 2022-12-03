using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public interface IPagging
    {
        uint PageActive { get; }
        uint PageSize { get; }
        uint PageStart { get; }
        uint GetPageEnd();
    }
}
