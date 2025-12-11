using Microsoft.AspNetCore.Components;
using Sufficit.Telephony.EventsPanel;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public partial class CardChannels : ComponentBase, IDisposable
    {

        #region PARAMETERS

        [Parameter]
        public EventsPanelCardKind Kind { get; set; }

        [Parameter]
        public ChannelInfoCollection? Channels { get; set; }

        [Parameter]
        public string? ClientCode { get; set; }

        [CascadingParameter]
        private Panel? Panel { get; set; }

        #endregion
        #region METHODS

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (Channels != null)
            {
                Channels.OnChanged += Channels_OnChanged;
            }
        }

        private async void Channels_OnChanged(IMonitor? monitor, object? state)
        {
            await InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// Get the channels to display, applying filters and limits based on configuration
        /// </summary>
        protected ChannelInfoCollection GetChannelsToDisplay()
        {
            if (Channels == null || !Channels.Any())
                return new ChannelInfoCollection();

            // For queues, apply MaxQueueCalls limit
            if (Kind == EventsPanelCardKind.QUEUE)
            {
                var maxCalls = Panel?.Options?.MaxQueueCalls ?? 5;

                // If maxCalls is 0, show all
                if (maxCalls <= 0)
                    return Channels;

                // Order by oldest first (longest waiting time) and take the limit
                var limitedChannels = Channels
         .OrderBy(c => c.Content?.Start ?? System.DateTime.MaxValue)
                  .Take(maxCalls);

                var result = new ChannelInfoCollection();
                foreach (var channel in limitedChannels)
                {
                    result.Add(channel);
                }
                return result;
            }

            // For other card types, return all channels
            return Channels;
        }

        public void Dispose()
        {
            if (Channels != null)
            {
                Channels.OnChanged -= Channels_OnChanged;
            }
        }

        #endregion

    }
}
