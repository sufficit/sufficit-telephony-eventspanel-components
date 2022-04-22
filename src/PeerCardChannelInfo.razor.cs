using Microsoft.AspNetCore.Components;
using Sufficit.Asterisk;
using Sufficit.Telephony.EventsPanel;
using System.Threading.Tasks;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public partial class PeerCardChannelInfo
    {
        [Parameter]
        public ChannelInfoMonitor? Channel { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (Channel != null)
            {
                Channel.Changed += Channel_Changed;
            }
        }

        private async void Channel_Changed(object? sender, AsteriskChannelState e)
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}
