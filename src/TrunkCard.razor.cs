using Microsoft.AspNetCore.Components;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public partial class TrunkCard : EventsPanelCardAbstract
    {
        public new EventsPanelTrunkCard Card { get => (EventsPanelTrunkCard)base.Card; set => base.Card = value; }

        protected PeerInfo? Content => Card.Monitor?.Content;

        protected string? PeerKey 
        {
            get {
                if (Content != null)
                    return $"Peer: { ((IKey)Content).Key }";

                return null;
            } 
        }
    }
}
