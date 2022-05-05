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
    public partial class QueueCard
    {
        [Parameter]
        public EventsPanelCardMonitor<QueueInfoMonitor>? Monitor { get; set; }

        [Parameter]
        public string? Avatar { get; set; }

        [Parameter]
        public Task<string>? HandleAvatar { get; set; }

        [CascadingParameter]
        public EventsPanel? Panel { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (Monitor != null)
            {
                Monitor.OnChanged += Changed;

                if(HandleAvatar != null)               
                    Avatar = await HandleAvatar;                
            }
        }

        protected QueueInfo Content => Monitor?.Content!;

        private async void Changed(IMonitor? monitor, object? state)
        {            
            if(state is QueueCallerLeaveEvent leave)
            {                              
                Monitor?.Channels.Remove(leave.Channel);                
            }

            await InvokeAsync(StateHasChanged);
        }

        protected string GetAvatarSrc()
        {
            if (!string.IsNullOrWhiteSpace(Avatar))
                return Avatar;

            return $"/_content/{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}/assets/img/siluet.png";
        }

        protected async Task OnRefreshClicked(MouseEventArgs _)
        {
            if(Panel != null && Panel.Service != null)
            {
                await Panel.Service.GetQueueStatus(Content.Key, string.Empty);
            } else { Console.WriteLine("problem on send action"); }
        }
    }
}
