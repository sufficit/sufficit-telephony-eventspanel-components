using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Sufficit.Telephony.EventsPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public partial class EventsPanelCardArea : EventsPanelView
    {
        [Inject]
        public EventsPanelService Service { get; internal set; } = default!;

        [CascadingParameter]
        public Panel Panel { get; internal set; } = default!;        

        [Parameter]
        public PaggingControl Pagging { get; set; } = default!;

        [Parameter]
        public FilteringControl? Filtering { get; set; }

        public IEventsPanelCardCollection Cards => Panel.Cards;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            Cards.OnChanged += (_, _) => ShouldRefresh();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                // Imediately apply visual changes
                Pagging.OnPaggingChanged += (_) => StateHasChanged();
                
                if(Filtering != null)
                    Filtering.OnFilterChanged += (_) => StateHasChanged();
            }
        }
                                
        protected async Task<string> GetAvatar(EventsPanelCard monitor)
        {
            if(Service?.CardAvatarHandler != null)
            {
                return await Service.CardAvatarHandler.Invoke(monitor);
            }

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

        protected IEnumerable<EventsPanelTrunkCard> GetTrunks()
        {
            string? filter = Filtering?.FilterText;
            Func<EventsPanelTrunkCard, bool>? whereClause = (_) => true;
            if (!string.IsNullOrWhiteSpace(filter))
                whereClause = (EventsPanelTrunkCard s) => s.IsMatchFilter(filter);

            return Cards.Trunks.Where(whereClause).OrderBy(s => s.Info.Order).ThenBy(s => s.Info.Label);
        }
    }
}
