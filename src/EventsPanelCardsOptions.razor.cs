using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public partial class EventsPanelCardsOptions : ComponentBase
    {
        [Parameter]
        public PaggingControl? Pagging { get; set; }

        [CascadingParameter]
        public PaggingContext? PaggingContext { get; set; }

        [Parameter]
        public uint? Total { get; set; }

        private async Task PaggingSelectChanged(uint pageSize)
        {
            if (Pagging != null)
            {
                Pagging.SetPageSize(pageSize);
                await InvokeAsync(StateHasChanged);
            }
        }
    }
}
