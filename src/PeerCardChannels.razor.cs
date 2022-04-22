using Microsoft.AspNetCore.Components;
using Sufficit.Telephony.EventsPanel;
using System.Threading.Tasks;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public partial class PeerCardChannels
    {
        [Parameter]
        public ChannelInfoCollection? Channels { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (Channels != null)
            {
                Channels.CollectionChanged += Channels_CollectionChanged;
            }
        }

        private async void Channels_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}
