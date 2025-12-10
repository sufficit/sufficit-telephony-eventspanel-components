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

        // Estado de expansão das filas - inicializa como true (expandido)
        protected bool QueuesExpanded { get; set; } = true;

        // Calcula dinamicamente baseado nas coleções reais
        protected bool OnlyPeers
        {
            get
            {
                if (Cards.OnlyPeers.GetValueOrDefault())
                    return true;

                // Calcula os totais reais da coleção
                var queuesCount = Cards.Queues.Count();
                var trunksCount = Cards.Trunks.Count();

                return queuesCount == 0 && trunksCount == 0;
            }
        }

        protected int QueuesTotal;
        protected int TrunksTotal;
        
        private IEnumerable<EventsPanelQueueCard>? _cachedQueues;
        private IEnumerable<EventsPanelTrunkCard>? _cachedTrunks;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            Cards.OnChanged += OnCardsChanged;

            // Imediately apply visual changes
            Pagging.OnPaggingChanged += OnPaggingChanged;

            if (Filtering != null)
                Filtering.OnFilterChanged += OnFilteringChanged;

            // Calculate counts before rendering
            UpdateCachedCollections();
        }

        private void UpdateCachedCollections()
        {
            // Cache queues and calculate total
            _cachedQueues = CalculateQueues();
            QueuesTotal = _cachedQueues.Count();

            // Cache trunks and calculate total
            _cachedTrunks = CalculateTrunks();
            TrunksTotal = _cachedTrunks.Count();
        }

        public override void Dispose(bool disposing)
        {
            Cards.OnChanged -= OnCardsChanged;
            Pagging.OnPaggingChanged -= OnPaggingChanged;

            if (Filtering != null)
                Filtering.OnFilterChanged -= OnFilteringChanged;
        }

        protected void OnCardsChanged(EventsPanelCard? _, NotifyCollectionChangedAction __)
        {
            UpdateCachedCollections();
            ShouldRefresh();
        }

        protected async void OnPaggingChanged(IPagging? _)
            => await InvokeAsync(StateHasChanged);

        protected async void OnFilteringChanged(string? _)
        {
            UpdateCachedCollections();
            await InvokeAsync(StateHasChanged);
        }
    
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

        private IEnumerable<EventsPanelQueueCard> CalculateQueues()
        {
            string? filter = Filtering?.FilterText;
            Func<EventsPanelQueueCard, bool>? whereClause = (_) => true;
            if (!string.IsNullOrWhiteSpace(filter))
                whereClause = (EventsPanelQueueCard s) => s.IsMatchFilter(filter);

            var queues = Cards.Queues.Where(whereClause);
    
            return queues;
        }

        protected IEnumerable<EventsPanelQueueCard> GetQueues()
        {
         // Return cached collection
            return _cachedQueues ?? Enumerable.Empty<EventsPanelQueueCard>();
        }

        private IEnumerable<EventsPanelTrunkCard> CalculateTrunks()
        {
  string? filter = Filtering?.FilterText;
  Func<EventsPanelTrunkCard, bool>? whereClause = (_) => true;
  if (!string.IsNullOrWhiteSpace(filter))
        whereClause = (EventsPanelTrunkCard s) => s.IsMatchFilter(filter);

  var result = Cards.Trunks.Where(whereClause).OrderBy(s => s.Info.Order).ThenBy(s => s.Info.Label);
            return result;
        }

    protected IEnumerable<EventsPanelTrunkCard> GetTrunks()
        {
            // Return cached collection
            return _cachedTrunks ?? Enumerable.Empty<EventsPanelTrunkCard>();
        }

        // Método para alternar expansão/colapso
        protected void ToggleQueues()
        {
            QueuesExpanded = !QueuesExpanded;
        }

        // Retorna o estilo do card de filas baseado no estado de expansão
        protected string GetQueuesCardStyle()
        {
            return QueuesExpanded 
                ? "padding-bottom: 0.5rem !important;" 
                : "padding-bottom: 0.25rem !important;";
        }
    }
}
