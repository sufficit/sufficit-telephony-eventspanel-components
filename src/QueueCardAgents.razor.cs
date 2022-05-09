using Microsoft.AspNetCore.Components;
using Sufficit.Asterisk;
using Sufficit.Asterisk.Events;
using Sufficit.Blazor.UI.Material;
using Sufficit.Telephony.EventsPanel;
using System.Threading.Tasks;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public partial class QueueCardAgents
    {
        [Parameter]
        public GenericCollection<QueueAgentInfo>? Items { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (Items != null)
            {
                Items.OnChanged += Channels_OnChanged;
            }
        }

        private async void Channels_OnChanged(IMonitor? monitor, object? state)
        {
            if(monitor != null)
                monitor.OnChanged += AgentChanged;

            await InvokeAsync(StateHasChanged);
        }

        private async void AgentChanged(IMonitor? sender, object? state)
        {
            await InvokeAsync(StateHasChanged);
        }

        protected Animations? GetAnimations(QueueAgentInfo? info)
        {
            if (info != null && info.Status.HasFlag(AsteriskDeviceStatus.Ringing))
                return Animations.Blink;
            return null;
        }
        protected string GetIconKey(QueueAgentInfo? info)
        {
            switch (info?.Status)
            {
                case AsteriskDeviceStatus.InUse: return "supervised_user_circle";
                case AsteriskDeviceStatus.Unavailable: return "no_accounts";
                default: return "account_circle";
            }
        }
    }
}
