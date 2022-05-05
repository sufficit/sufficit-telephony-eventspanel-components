using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Sufficit.Telephony.EventsPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public partial class EventsPanel
    {
        [Inject]
        public EventsPanelService? Service { get; internal set; }

        [Parameter]
        public IEnumerable<EventsPanelCardMonitor>? Cards
        {
            get
            {
                return Service?.Panel.Cards;
            }
            set
            {
                Service?.Panel.Cards.Clear();
                if (value != null)
                {
                    foreach (var monitor in value)
                        Service?.Panel.Cards.Add(monitor);
                }
            }
        }

        protected Exception? Exception { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (Service != null)
            {
                Service.OnChanged += Service_OnChanged;
                Service.Panel.Cards.OnChanged += CardsOnChanged;
            }
        }

        private async void Service_OnChanged()
        {
            try
            {
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex) { Exception = ex; }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender && Service != null && Service.IsConfigured)
            {
                try
                {
                    if (Service != null)
                        await Service.StartAsync(System.Threading.CancellationToken.None);

                    Exception = null;
                }
                catch (Exception ex)
                {
                    Exception = ex;
                }

                await InvokeAsync(StateHasChanged);
            }
        }

        private async void CardsOnChanged(IMonitor? monitor, object? state)
        {
            try
            {
                await InvokeAsync(StateHasChanged);
            } catch(Exception ex) { Exception = ex; }
        }

        protected async Task Refresh()
        {
            if(Service != null && Service.IsConnected)
                await Service.GetPeerStatus();
        }

        protected bool HasCards() => Service?.Panel.Cards.Any() ?? false;

        protected IEnumerable<EventsPanelCardMonitor<PeerInfoMonitor>> Trunks
            => Service?.Panel.Cards.ToList<EventsPanelCardMonitor<PeerInfoMonitor>>()
                .Where(s => s.Card.Kind == EventsPanelCardKind.TRUNK) ?? Array.Empty<EventsPanelCardMonitor<PeerInfoMonitor>>();

        protected IEnumerable<EventsPanelCardMonitor<PeerInfoMonitor>> Peers
            => (Service?.Panel.Cards.ToList<EventsPanelCardMonitor<PeerInfoMonitor>>()
              .Where(s => s.Card.Kind == EventsPanelCardKind.PEER) ?? Array.Empty<EventsPanelCardMonitor<PeerInfoMonitor>>())
              .OrderBy(s => s.Card.Label);

        protected IEnumerable<EventsPanelCardMonitor<QueueInfoMonitor>> Queues
           => (Service?.Panel.Cards.ToList<EventsPanelCardMonitor<QueueInfoMonitor>>()
             .Where(s => s.Card.Kind == EventsPanelCardKind.QUEUE) ?? Array.Empty<EventsPanelCardMonitor<QueueInfoMonitor>>())
             .OrderBy(s => s.Card.Label);

        protected async Task<string> GetAvatar(EventsPanelCardMonitor monitor)
        {
            if(Service?.CardAvatarHandler != null)
            {
                return await Service.CardAvatarHandler.Invoke(monitor);
            }

            return string.Empty;
        }

        protected int MaxButtons => Service?.Options?.MaxButtons ?? 0;
    }
}
