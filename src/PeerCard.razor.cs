using Microsoft.AspNetCore.Components;
using Sufficit.Asterisk;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public partial class PeerCard : EventsPanelCardAbstract<EventsPanelPeerCard>
    {
        protected PeerInfo? Content => Card.Monitor?.Content;

        protected string? PeerKey 
        {
            get {
                if(Content != null)                
                    return ((IKey)Content).Key;
                
                return null;
            } 
        }

        protected string GetLabel() => !string.IsNullOrWhiteSpace(Card.Label) ? Card.Label : PeerKey ?? "Desconhecido";

        protected string GetCardStyle()
        {
            string val = "";
            
            if (Card.Channels.Any())            
                val = "background: rgba(255,150,0,.1); ";            

            return val;
        }
    }
}
