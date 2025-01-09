using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public partial class EventsPanelStatsFilter
    {
        [Parameter]
        [EditorRequired]
        public FilteringControl Filtering { get; set; } = default!;
    }
}
