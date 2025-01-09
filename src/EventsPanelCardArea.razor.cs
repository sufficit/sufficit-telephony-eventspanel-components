using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Sufficit.Telephony.EventsPanel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public partial class EventsPanelCardArea : EventsPanelView
    {
        [Inject]
        public EventsPanelService Service { get; internal set; } = default!;

        [EditorRequired]
        [Parameter]
        public IEventsPanelCardCollection Cards { get; set; } = default!;

        [EditorRequired]
        [Parameter]
        public PaggingControl Pagging { get; set; } = default!;

        [Parameter]
        public FilteringControl? Filtering { get; set; }

        protected bool OnlyPeers => Cards.OnlyPeers.GetValueOrDefault() ? true : (TrunksTotal == 0 && QueuesTotal == 0);

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            Cards.OnChanged += OnCardsChanged;

            // Imediately apply visual changes
            Pagging.OnPaggingChanged += OnPaggingChanged;

            if (Filtering != null)
                Filtering.OnFilterChanged += OnFilteringChanged;
        }

        public override void Dispose(bool disposing)
        {
            Cards.OnChanged -= OnCardsChanged;
            Pagging.OnPaggingChanged -= OnPaggingChanged;

            if (Filtering != null)
                Filtering.OnFilterChanged -= OnFilteringChanged;
        }

        protected void OnCardsChanged(EventsPanelCard? _, NotifyCollectionChangedAction __)
            => ShouldRefresh();

        protected async void OnPaggingChanged(IPagging? _)
            => await InvokeAsync(StateHasChanged);

        protected async void OnFilteringChanged(string? _)
            => await InvokeAsync(StateHasChanged);
                                        
        protected async Task<string> GetAvatar(EventsPanelCard monitor)
        {
            if (Cards?.CardAvatarHandler != null)            
                return await Cards.CardAvatarHandler.Invoke(monitor);            

            return string.Empty;
        }

        protected IEnumerable<EventsPanelPeerCard> GetPeers()
        {
            string? filter = Filtering?.FilterText;
            Func<EventsPanelPeerCard, bool>? whereClause = (_) => true;
            if(!string.IsNullOrWhiteSpace(filter)) 
                whereClause = (EventsPanelPeerCard s) => s.IsMatchFilter(filter);

            var peers = Cards.Peers.Where(whereClause);
            Pagging.PageTotalItems = (uint)peers.Count();

            if (Pagging.PageSize > 0)            
                return peers.Take((int)Pagging.PageSize);            
            else            
                return peers;            
        }

        protected int QueuesTotal;
        protected IEnumerable<EventsPanelQueueCard> GetQueues()
        {
            string? filter = Filtering?.FilterText;
            Func<EventsPanelQueueCard, bool>? whereClause = (_) => true;
            if (!string.IsNullOrWhiteSpace(filter))
                whereClause = (EventsPanelQueueCard s) => s.IsMatchFilter(filter);

            var result = Cards.Queues.Where(whereClause);
            QueuesTotal = result.Count();
            return result;
        }

        protected int TrunksTotal;
        protected IEnumerable<EventsPanelTrunkCard> GetTrunks()
        {
            string? filter = Filtering?.FilterText;
            Func<EventsPanelTrunkCard, bool>? whereClause = (_) => true;
            if (!string.IsNullOrWhiteSpace(filter))
                whereClause = (EventsPanelTrunkCard s) => s.IsMatchFilter(filter);

            var result = Cards.Trunks.Where(whereClause).OrderBy(s => s.Info.Order).ThenBy(s => s.Info.Label);
            TrunksTotal = result.Count();
            return result;
        }
    }
}
