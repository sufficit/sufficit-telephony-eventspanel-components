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

        protected string GetCardClass()
        {
            // Verifica se há canais chamando
            bool hasCallingChannel = Card.Channels.Any(monitor => IsChannelCalling(monitor));

            if (hasCallingChannel)
                return "peer-calling";

            if (Content?.Status == PeerStatus.Registered)
                return "peer-registered";
  
            else if (Content?.Status == PeerStatus.Unknown)
                return "peer-unknown";
            else if (Content?.Status == PeerStatus.Unregistered)
                return "peer-unregistered";

            return "";
        }

        protected bool IsChannelCalling(ChannelInfoMonitor monitor)
        {
            var content = monitor.GetContent();
            
            // Um canal está em chamada se:
            // 1. Não foi desligado (Hangup == null)
            // 2. Está em um estado ativo (Up, Ringing, Dialing, Busy, etc.)
            if (content?.Hangup != null)
                return false;

            return content?.State is 
                AsteriskChannelState.Up or 
                AsteriskChannelState.Ringing or 
                AsteriskChannelState.Ring or 
                AsteriskChannelState.Dialing or 
                AsteriskChannelState.Busy or 
                AsteriskChannelState.DialingOffhook or 
                AsteriskChannelState.OffHook or 
                AsteriskChannelState.PreRing or
                AsteriskChannelState.Down;
        }
    }
}
