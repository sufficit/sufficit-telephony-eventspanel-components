﻿using Microsoft.AspNetCore.Components;
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
                Channels.OnChanged += Channels_OnChanged;
            }
        }

        private async void Channels_OnChanged(IMonitor? monitor, object? state)
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}
