using Microsoft.AspNetCore.Components;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public abstract class EventsPanelCardAbstract<T> : ComponentBase, IDisposable where T : EventsPanelCard
    {
        [Parameter, EditorRequired]
        public virtual T Card { get; set; } = default!;

        [Parameter]
        public string? Avatar { get; set; }

        [Parameter]
        public Task<string>? HandleAvatar { get; set; }

        /// <summary>
        ///     Indicates that this object is active in use by frontend
        /// </summary>
        public bool IsRendered { get; internal set; }

        protected override async Task OnParametersSetAsync()
        {
            if (HandleAvatar != null)
                Avatar = await HandleAvatar;

            if (Card.Monitor != null)
            {
                // ensure a single handler
                Card.Monitor.OnChanged -= OnMonitorChanged;
                Card.Monitor.OnChanged += OnMonitorChanged;
            }

            Card.Channels.OnChanged -= ChannelsChanged;
            Card.Channels.OnChanged += ChannelsChanged;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            IsRendered = true;
        }

        public void Dispose()
        {
            IsRendered = false;
            Console.WriteLine($"disposing card, for: {Card.Monitor?.Key}");

            if (Card.Monitor != null)            
                Card.Monitor.OnChanged -= OnMonitorChanged;            

            Card.Channels.OnChanged -= ChannelsChanged;
        }

        /// <summary>
        /// On Channels changed
        /// </summary>
        protected virtual async void ChannelsChanged(ChannelInfoMonitor? sender, object? state)
        {
            if (IsRendered) 
                await InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// On Underlaying Monitor Changed
        /// </summary>
        protected virtual async void OnMonitorChanged(IMonitor? sender, object? state)
        {
            if (IsRendered)
            {
                Console.WriteLine($"event received: {state}, for: {Card.Monitor?.Key}");
                await InvokeAsync(StateHasChanged);
            }
        }

        protected string GetAvatarSrc()
        {
            if (!string.IsNullOrWhiteSpace(Avatar))
                return Avatar;

            return $"/_content/{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}/assets/img/siluet.png";
        }
    }
}
