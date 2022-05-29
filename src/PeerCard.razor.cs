using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public partial class PeerCard : EventsPanelCardAbstract
    {
        public new EventsPanelPeerCard Card { get => (EventsPanelPeerCard)base.Card; set => base.Card = value; }

        protected PeerInfo? Content => Card.Monitor?.Content;

        protected string? PeerKey 
        {
            get {
                if(Content != null)                
                    return ((IKey)Content).Key;
                
                return null;
            } 
        }
    }
}
