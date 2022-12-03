using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sufficit.Telephony.EventsPanel.Components.IPaggingMonitor;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public class PaggingControl : IPagging, IPaggingMonitor
    {
        public uint PageActive { get; set; } = 1;

        public uint PageSize { get; internal set; }

        public uint PageStart { get; internal set; }

        public uint PageTotalItems { get; set; }

        public uint GetPageEnd() => PageStart + PageSize;


        public event PaggingChanged? OnPaggingChanged;

        public void SetPageSize(uint pageSize)
        {
            if(PageSize != pageSize)
            {
                PageSize = pageSize;
                OnPaggingChanged?.Invoke(this);                
            }
        }

        public uint GetPages() => GetPages(PageTotalItems);

        public uint GetPages(uint TotalItems)
        {
            if (TotalItems == 0 || PageSize == 0) return 0;

            return (uint)Math.Ceiling((double)TotalItems / PageSize);
        }
    }
}
