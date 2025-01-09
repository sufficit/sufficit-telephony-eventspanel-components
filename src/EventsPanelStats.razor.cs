using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public partial class EventsPanelStats : EventsPanelView
    {
        [Inject]
        public EventsPanelService Service { get; internal set; } = default!;

        [CascadingParameter]
        protected Panel Panel { get; set; } = default!;

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                Service.OnChanged += OnEPSChanged;
                Service.Channels.OnChanged += OnEPSChannelsChanged;                
            }
        }

        private void OnEPSChanged(HubConnectionState? _, Exception? __)
            => ShouldRefresh();

        private void OnEPSChannelsChanged(ChannelInfoMonitor? _, object? __)
            => ShouldRefresh();

        public override void Dispose(bool disposing)
        {
            Service.OnChanged -= OnEPSChanged;
            Service.Channels.OnChanged -= OnEPSChannelsChanged;
        }

        protected async Task Refresh()
        {
            if(Service != null && Service.IsConnected)
                await Service.GetPeerStatus();
        }

        protected int MaxButtons => Panel.Options?.MaxButtons ?? 0;
    }
}
