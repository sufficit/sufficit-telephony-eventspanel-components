using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public interface IPaggingMonitor
    {
        delegate void PaggingChanged(IPagging? sender);

        event PaggingChanged? OnPaggingChanged;

        void SetPageSize(uint pageSize);
    }
}
