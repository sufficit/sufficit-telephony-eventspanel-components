using Microsoft.AspNetCore.Components;
using Sufficit.Asterisk;
using Sufficit.Telephony.EventsPanel;
using System.Threading.Tasks;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public partial class PeerCardChannelInfo
    {
        [Parameter]
        public ChannelInfoMonitor? Monitor { get; set; }

        protected ChannelInfo Content => Monitor?.GetContent()!;

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (Monitor != null)
            {
                Monitor.OnChanged += ChannelOnChanged;
            }
        }

        private async void ChannelOnChanged(IMonitor? sender, object? state) 
            => await InvokeAsync(StateHasChanged);

        protected string? Animation { get; set; }

        protected string GetIconKey()
        {
            Animation = string.Empty;
            if(Monitor != null)
            {
                switch (Content.State)
                {
                    case AsteriskChannelState.Up: return EventsPanelDefaults.ICON_STATE_UP;
                    case AsteriskChannelState.Ringing: return EventsPanelDefaults.ICON_STATE_RINGING;
                    case AsteriskChannelState.Ring: return EventsPanelDefaults.ICON_STATE_RING;
                    case AsteriskChannelState.Down: return EventsPanelDefaults.ICON_STATE_DOWN;
                    case AsteriskChannelState.Dialing: return EventsPanelDefaults.ICON_STATE_DIALING;
                    case AsteriskChannelState.Busy: return EventsPanelDefaults.ICON_STATE_BUSY;
                    case AsteriskChannelState.Unknown: return EventsPanelDefaults.ICON_STATE_UNKNOWN;
                }
            }
            return "info";
        }
    }
}
