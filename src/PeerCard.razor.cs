using Microsoft.AspNetCore.Components;
using Sufficit.Asterisk;
using Sufficit.Asterisk.Events;
using Sufficit.Telephony.EventsPanel;
using System.Threading.Tasks;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public partial class PeerCard
    {
        [Parameter]
        public EventsPanelCardMonitor? Peer { get; set; }

        [Parameter]
        public string? Avatar { get; set; }

        [Parameter]
        public Task<string>? HandleAvatar { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (Peer != null)
            {
                Peer.Changed += Peer_Changed;

                if(HandleAvatar != null)               
                    Avatar = await HandleAvatar;                
            }
        }

        private async void Peer_Changed(object? sender, PeerStatus e)
        {
            await InvokeAsync(StateHasChanged);
        }

        protected string GetAvatarSrc()
        {
            if (!string.IsNullOrWhiteSpace(Avatar))
                return Avatar;

            return $"/_content/{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}/assets/img/siluet.png";
        }
    }
}
