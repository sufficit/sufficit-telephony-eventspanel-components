using Microsoft.AspNetCore.Components;
using Sufficit.Blazor.UI.Material;
using Sufficit.Blazor.UI.Material.Components;
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
        public PaggingControl Pagging { get; set; } = default!;

        [CascadingParameter]
        public PaggingContext PaggingContext { get; set; } = default!;

        [Parameter]
        public uint? Total { get; set; }

        private async void PaggingSelectChanged(SelectedChangedEventArgs<string?> e)
        {
            if (uint.TryParse(e.Current, out uint pagesize))
            {
                PaggingContext.Monitor?.SetPageSize(pagesize);
                await InvokeAsync(StateHasChanged);
            }
        }

    }
}
