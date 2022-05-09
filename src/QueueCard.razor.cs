using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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

        protected override void CardChanged(IMonitor? monitor, object? state)
        {
            if (state is QueueCallerLeaveEvent leave)
            {
                Card.Channels.Remove(leave.Channel);
            }

            base.CardChanged(monitor, state);
        }

        protected async Task OnRefreshClicked(MouseEventArgs _)
        {            
            if(Content != null)
                await Service.GetQueueStatus(Content.Key, string.Empty);           
        }
    }
}
