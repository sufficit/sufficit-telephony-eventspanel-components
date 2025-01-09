using MudBlazor;
using Sufficit.Asterisk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public static class UIExtensions
    {
        public static Color ToColor(this PeerStatus source)
        {
            return source switch
            {
                PeerStatus.Unregistered => Color.Error,
                PeerStatus.Lagged => Color.Warning,
                PeerStatus.Reachable => Color.Success,
                PeerStatus.Unreachable => Color.Error,
                PeerStatus.Rejected => Color.Error,
                PeerStatus.Registered => Color.Success,
                PeerStatus.Unmonitored => Color.Info,
                PeerStatus.Ok => Color.Success,
                _ => Color.Default,
            };
        }
    }
}
