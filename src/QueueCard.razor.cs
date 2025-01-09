using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Sufficit.Asterisk;
using Sufficit.Asterisk.Manager.Events;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public partial class QueueCard : EventsPanelCardAbstract<EventsPanelQueueCard>
    {
        [Inject]
        public EventsPanelService Service { get; internal set; } = default!;

        [Parameter]
        public bool CanToggle { get; set; } = true;

        protected QueueInfo? Content => Card.Monitor?.Content;

        protected override void OnMonitorChanged(IMonitor? monitor, object? state)
        {
            if (state is QueueCallerLeaveEvent leave)            
                Card.Channels.Remove(leave.Channel);            

            base.OnMonitorChanged(monitor, state);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Service.OnChanged += ServiceChanged;
        }

        private async void ServiceChanged(HubConnectionState? state, Exception? ex)
        {
             if(state.HasValue && state.Value == HubConnectionState.Connected)            
                if(!CanToggle) // only if has a few queues showing
                    await RequestStatusEvents();            
        }

        protected async Task OnRefreshClicked(MouseEventArgs _)
            => await RequestStatusEvents();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                if (!CanToggle) // only if has a few queues showing
                    await RequestStatusEvents();
            }
        }

        protected async Task RequestStatusEvents()
        {
            if (Content != null && Service.IsConnected)
                await Service.GetQueueStatus(Content.Key, string.Empty);
        }

        protected bool ShowAgents => (!CanToggle || Active) && Card.IsMonitored && Card.Content!.Agents.Any();

        protected bool Active { get; set; }

        public void Toggle()
        {
            Active = !Active;
            StateHasChanged();
        }

        protected void OnToggleClicked(MouseEventArgs _)
            => Toggle();

        protected String GetCardStyle()
        {
            string val = "width: 233.6px; background: rgba(180,240,255,.1);";

            if (Card.Channels.Any()) { }

            if (Card.IsMonitored) { }

            return val;
        }
    }
}
