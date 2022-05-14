using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Sufficit.Asterisk;
using Sufficit.Asterisk.Events;
using Sufficit.Asterisk.Manager.Events;
using Sufficit.Telephony.EventsPanel;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public partial class QueueCard : EventsPanelCardAbstract
    {
        public new EventsPanelQueueCard Card { get => (EventsPanelQueueCard)base.Card; set => base.Card = value; }

        [Inject]
        public EventsPanelService Service { get; internal set; } = default!;

        protected QueueInfo? Content => Card.Monitor?.Content;

        protected override void ChannelsChanged(IMonitor? monitor, object? state)
        {
            if (state is QueueCallerLeaveEvent leave)
            {
                Card.Channels.Remove(leave.Channel);
            }

            base.ChannelsChanged(monitor, state);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Service.OnChanged += ServiceChanged;
        }

        private async void ServiceChanged(HubConnectionState? state, Exception? ex)
        {
            if(state.HasValue && state.Value == HubConnectionState.Connected)            
                await RequestStatusEvents();            
        }

        protected async Task OnRefreshClicked(MouseEventArgs _)
            => await RequestStatusEvents();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender) await RequestStatusEvents();            
        }

        protected async Task RequestStatusEvents()
        {
            if (Content != null && Service.IsConnected)
                await Service.GetQueueStatus(Content.Key, string.Empty);
        }
    }
}
