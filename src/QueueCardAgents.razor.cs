using Microsoft.AspNetCore.Components;
using Sufficit.Asterisk;
using Sufficit.Asterisk.Events;
using Sufficit.Telephony.EventsPanel;
using System.Threading.Tasks;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public partial class QueueCardAgents
    {
        [Parameter]
        public GenericCollection<QueueAgentInfo>? Items { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (Items != null)
            {
                Items.OnChanged += Channels_OnChanged;
            }
        }

        private async void Channels_OnChanged(IMonitor? monitor, object? state)
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}
