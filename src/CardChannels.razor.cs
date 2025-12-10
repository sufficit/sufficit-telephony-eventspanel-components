using Microsoft.AspNetCore.Components;
using Sufficit.Telephony.EventsPanel;
using System.Threading.Tasks;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public partial class CardChannels
    {
        [Parameter]
        public EventsPanelCardKind Kind { get; set; }

        [Parameter]
        public ChannelInfoCollection? Channels { get; set; }

        [Parameter]
        public string? ClientCode { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (Channels != null)
            {
                Channels.OnChanged += Channels_OnChanged;
            }
        }

        private async void Channels_OnChanged(IMonitor? monitor, object? state)
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}
