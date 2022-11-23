using Microsoft.AspNetCore.Components;
using static MudBlazor.CategoryTypes;
using Sufficit.Asterisk;

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

        protected String GetCardStyle()
        {
            string val = "width: 260px; ";

            if (Card.Channels.Any())
            {
                val += "background: rgba(255,150,0,.1); ";
            }

            if (Card.IsMonitored && this.Content != null)
            {
                switch (this.Content.Status)
                {
                    case PeerStatus.Registered:
                        val += "opacity: 1";
                        break;
                    case PeerStatus.Unregistered:
                        val += "opacity: 0.7";
                        break;
                    case PeerStatus.Unknown:
                        val += "opacity: 0.5";
                        break;
                    default:
                        val += "opacity: 0";
                        break;
                }
            }
            else
            {
                val += "opacity: 0";
            }
            return val;
        }
    }

}
