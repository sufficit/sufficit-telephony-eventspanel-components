using Microsoft.AspNetCore.Components;
using Sufficit.Asterisk;
using Sufficit.Blazor;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public partial class PeerCard : EventsPanelCardAbstract<EventsPanelPeerCard>
    {
        protected PeerInfo? Content => Card.Monitor?.Content;

        protected string? PeerKey
        {
            get
            {
                if (Content != null)
                    return ((IKey)Content).Key;

                return null;
            }
        }

        protected string GetLabel() => !string.IsNullOrWhiteSpace(Card.Label) ? Card.Label : PeerKey ?? "Desconhecido";

        protected string GetCardStyle()
        {


            // Verifica se há canais chamando
            bool hasCallingChannel = Card.Channels.Any(monitor => IsChannelCalling(monitor));

            if (hasCallingChannel)
                return "background: rgba(var(--mud-palette-secondary-rgb), 0.2); ";

            if (Content?.Status == PeerStatus.Registered)
                return "background: rgba(var(--mud-palette-success-rgb), 0.2); ";
  
            else if (Content?.Status == PeerStatus.Unknown)
                return "opacity: .5;";
            else if (Content?.Status == PeerStatus.Unregistered)
                return string.Empty;

            return "";
        }

        protected bool IsChannelCalling(ChannelInfoMonitor monitor)
        {
            var content = monitor.GetContent();
            return content?.State is AsteriskChannelState.Down;
        }
    }
}
