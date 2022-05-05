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
                    case AsteriskChannelState.Up: return "phone_in_talk";
                    case AsteriskChannelState.Ringing: return "notifications_active";
                    case AsteriskChannelState.Ring: return "settings_phone";
                    case AsteriskChannelState.Down: return "call_end";
                    case AsteriskChannelState.Dialing: return "dialpad";
                    case AsteriskChannelState.Busy: return "phone_disabled";
                    case AsteriskChannelState.Unknown: return "contact_support";
                }
            }
            return "info";
        }
    }
}
