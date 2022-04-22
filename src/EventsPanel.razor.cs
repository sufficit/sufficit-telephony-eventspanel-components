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
        private EventsPanelService? Service { get; set; }

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
                Service.Panel.Cards.CollectionChanged += Cards_CollectionChanged;
            }
        }

        private async Task Service_OnChanged()
        {
            await InvokeAsync(StateHasChanged);
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

        private async void Cards_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await InvokeAsync(StateHasChanged);
        }

        protected async Task Refresh()
        {
            if(Service != null && Service.IsConnected)
                await Service.GetPeerStatus();
        }

        protected IEnumerable<EventsPanelCardMonitor> Trunks
            => Service?.Panel.Cards
                .Where(s => s.Kind == EventsPanelCardKind.TRUNK) ?? Array.Empty<EventsPanelCardMonitor>();

        protected IEnumerable<EventsPanelCardMonitor> Peers
            => (Service?.Panel.Cards
              .Where(s => s.Kind == EventsPanelCardKind.PEER) ?? Array.Empty<EventsPanelCardMonitor>())
              .OrderBy(s => s.Label);

        protected IEnumerable<EventsPanelCardMonitor> Queues
           => (Service?.Panel.Cards
             .Where(s => s.Kind == EventsPanelCardKind.QUEUE) ?? Array.Empty<EventsPanelCardMonitor>())
             .OrderBy(s => s.Label);

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
