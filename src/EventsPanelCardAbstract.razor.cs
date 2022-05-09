using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public abstract class EventsPanelCardAbstract : ComponentBase
    {
        [Parameter, EditorRequired]
        public virtual EventsPanelCard Card { get; set; } = null!;

        [Parameter]
        public string? Avatar { get; set; }

        [Parameter]
        public Task<string>? HandleAvatar { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            
            Card.OnChanged += CardChanged;
            if (Card.IsMonitored)
            {
                Card.Monitor!.OnChanged += MonitorChanged;
            }

            Card.Channels.OnChanged += ChannelsChanged;

            if (HandleAvatar != null)               
                Avatar = await HandleAvatar;  
        }

        /// <summary>
        /// On Card has changed
        /// </summary>
        /// <param name="monitor"></param>
        /// <param name="state"></param>
        protected virtual async void CardChanged(IMonitor? monitor, object? state)
        {
            await InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// On Channels changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="state"></param>
        protected virtual async void ChannelsChanged(IMonitor? sender, object? state)
        {
            await InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// On Undelaying Monitor Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="state"></param>
        protected virtual async void MonitorChanged(IMonitor? sender, object? state)
        {
            await InvokeAsync(StateHasChanged);
        }

        protected string GetAvatarSrc()
        {
            if (!string.IsNullOrWhiteSpace(Avatar))
                return Avatar;

            return $"/_content/{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}/assets/img/siluet.png";
        }
    }
}
