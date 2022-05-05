using Microsoft.AspNetCore.Components;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public partial class PeerCard
    {
        [Parameter]
        public EventsPanelCardMonitor<PeerInfoMonitor>? Monitor { get; set; }

        [Parameter]
        public string? Avatar { get; set; }

        [Parameter]
        public Task<string>? HandleAvatar { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (Monitor != null)
            {
                Monitor.OnChanged += Changed;

                if (HandleAvatar != null)               
                    Avatar = await HandleAvatar;                
            }
        }

        protected PeerInfo Content => Monitor?.Content!;

        protected bool IsMonitored => !string.IsNullOrWhiteSpace(((IKey)Content).Key);

        protected string? PeerKey 
        {
            get {
                if(Monitor?.Content != null)                
                    return $"Peer: { ((IKey)Content).Key }";
                
                return null;
            } 
        }

        private async void Changed(IMonitor? monitor, object? state)
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
