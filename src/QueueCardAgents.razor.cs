using Microsoft.AspNetCore.Components;
using Sufficit.Asterisk;
using MudBlazor;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public partial class QueueCardAgents : IDisposable
    {

        [Parameter]
        public MonitorCollection<QueueAgentInfo>? Items { get; set; }

        [Parameter]
        public string? ClientCode { get; set; }

        [CascadingParameter]
        public QueueCard? ParentCard { get; set; }

        private System.Timers.Timer? _updateTimer;

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (Items != null)
            {
                Items.OnChanged += OnCollectionChanged;
            }

            // Start timer to update call durations
            StartUpdateTimer();
        }

        private void StartUpdateTimer()
        {
            if (_updateTimer == null && Items != null)
            {
                _updateTimer = new System.Timers.Timer(1000); // Update every second
                _updateTimer.Elapsed += async (sender, e) =>
                   {
                       // Only update if there are agents in call or with InUse status
                       if (Items?.Any(a => a.InCall || a.Status == AsteriskDeviceStatus.InUse) == true)
                       {
                           await InvokeAsync(StateHasChanged);
                       }
                   };
                _updateTimer.Start();
            }
        }

        private void StopUpdateTimer()
        {
            if (_updateTimer != null)
            {
                _updateTimer.Stop();
                _updateTimer.Dispose();
                _updateTimer = null;
            }
        }

        private async void OnCollectionChanged(IMonitor? monitor, object? state)
        {
            // When a new agent is added or an existing one changes
            if (monitor != null)
            {
                // Subscribe to individual agent changes
                monitor.OnChanged += OnAgentChanged;
            }

            await InvokeAsync(StateHasChanged);
        }

        private async void OnAgentChanged(IMonitor? sender, object? state)
        {
            // When agent status changes (InCall, Paused, Status, etc.)
            await InvokeAsync(StateHasChanged);
        }

        protected string GetStatusTooltip(QueueAgentInfo agent)
        {
            if (agent.InCall)
                return $"Em chamada - {agent.Status}";
            if (agent.Paused)
                return $"Pausado - {agent.PausedReason ?? "Sem motivo"}";

            return agent.Status.ToString();
        }

        protected string GetCallDuration(QueueAgentInfo agent)
        {
            // Try to find an active channel for this agent
         var activeChannel = GetAgentActiveChannel(agent);
            if (activeChannel != null)
            {
                // Use channel start time for accurate duration
          var duration = DateTime.UtcNow - activeChannel.Start;
          if (activeChannel.Hangup != null)
  duration = activeChannel.Hangup.Timestamp - activeChannel.Start;

         if (duration.TotalHours >= 1)
            return duration.ToString(@"hh\:mm\:ss");

         return duration.ToString(@"mm\:ss");
  }

       // Fallback to LastCall if no active channel found
            // LastCall is a Unix timestamp in seconds
            if (agent.LastCall <= 0)
   return "00:00";

      // Calculate elapsed time since LastCall
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
     var elapsedSeconds = now - (long)agent.LastCall;

            // Validate range
    if (elapsedSeconds < 0 || elapsedSeconds > 86400) // More than 24 hours doesn't make sense
              return "00:00";

            var durationFromLastCall = TimeSpan.FromSeconds(elapsedSeconds);

            if (durationFromLastCall.TotalHours >= 1)
 return durationFromLastCall.ToString(@"hh\:mm\:ss");

            return durationFromLastCall.ToString(@"mm\:ss");
        }

        /// <summary>
      /// Finds the active channel for a specific agent by matching the interface
        /// </summary>
        private ChannelInfo? GetAgentActiveChannel(QueueAgentInfo agent)
        {
      if (ParentCard?.Card?.Channels == null)
   return null;

        // Extract the extension/number from agent interface
            // Format can be: "Local/6001@from-queue/n" or "PJSIP/6001" or "6001"
      string agentNumber = ExtractAgentNumber(agent.Interface);
  if (string.IsNullOrWhiteSpace(agentNumber))
  return null;

      // Find channel that matches this agent and is not hung up
            foreach (var channelMonitor in ParentCard.Card.Channels)
{
   var channel = channelMonitor.GetContent();
     if (channel?.Hangup == null)
         {
       // Check if channel Key (name) or CallerIdNum contains the agent number
       if (channel.Key?.Contains(agentNumber, StringComparison.OrdinalIgnoreCase) == true ||
              channel.CallerIdNum?.Contains(agentNumber, StringComparison.OrdinalIgnoreCase) == true)
       {
    return channel;
  }
             }
        }

  return null;
        }

  /// <summary>
        /// Extracts the number/extension from agent interface
      /// </summary>
        private string ExtractAgentNumber(string iface)
        {
        if (string.IsNullOrWhiteSpace(iface))
     return string.Empty;

       // Handle "Local/6001@from-queue/n" format
            if (iface.Contains("/"))
    {
     var parts = iface.Split('/');
      if (parts.Length >= 2)
          {
     var middlePart = parts[1];
  if (middlePart.Contains("@"))
   {
     return middlePart.Split('@')[0];
             }
   return middlePart;
 }
     }

       // Handle "PJSIP/6001" or just "6001"
            return iface;
 }

        /// <summary>
        /// Formats agent name/interface by removing client code prefix if present
        /// </summary>
        protected string FormatAgentDisplay(QueueAgentInfo agent)
        {
            var display = agent.Name ?? agent.Interface;

            if (string.IsNullOrWhiteSpace(display))
                return string.Empty;

            // Extract the number from formats like "Local/0006676007@from-queue/n"
            // or just "0006676007"
            string numberPart = display;

            // If contains "/", extract the part between "/" and "@"
            if (display.Contains("/"))
            {
                var parts = display.Split('/');
                if (parts.Length >= 2)
                {
                    // Get the middle part (e.g., "0006676007@from-queue")
                    var middlePart = parts[1];

                    // If contains "@", get only the part before it
                    if (middlePart.Contains("@"))
                    {
                        numberPart = middlePart.Split('@')[0];
                      }
                    else
                    {
                        numberPart = middlePart;
                      }
                }
            }

            // Now check if we should remove the client code prefix
            if (!string.IsNullOrWhiteSpace(ClientCode) && !string.IsNullOrWhiteSpace(numberPart))
            {
                // Check if the number part starts with the full client code (with zeros)
                if (numberPart.StartsWith(ClientCode, StringComparison.OrdinalIgnoreCase))
                {
                    // Remove the client code prefix and return (keeping zeros in extension)
                    return numberPart.Substring(ClientCode.Length);
                }
            }

            // If no client code to remove, return the full number
            return numberPart;
        }

        public void Dispose()
        {
            StopUpdateTimer();

            if (Items != null)
            {
                Items.OnChanged -= OnCollectionChanged;

                // Unsubscribe from all agent changes
                foreach (var agent in Items)
                {
                    agent.OnChanged -= OnAgentChanged;
                }
            }
        }

        /*
        protected Animations? GetAnimations(QueueAgentInfo? info)
        {
    if (info != null && info.Status.HasFlag(AsteriskDeviceStatus.Ringing))
          return Animations.Blink;
            return null;
        }
*/

        protected string GetIconKey(QueueAgentInfo? info)
        {
            switch (info?.Status)
            {
                case AsteriskDeviceStatus.InUse: return "supervised_user_circle";
                case AsteriskDeviceStatus.Unavailable: return "no_accounts";
                default: return "account_circle";
            }
        }

        protected string GetAgentStatusIcon(QueueAgentInfo agent)
        {
            if (agent.InCall)
                return Icons.Material.Filled.Phone;
            if (agent.Paused)
                return Icons.Material.Filled.PauseCircle;
            if (agent.Status == AsteriskDeviceStatus.InUse)
                return Icons.Material.Filled.PhoneEnabled;
            if (agent.Status == AsteriskDeviceStatus.Unavailable)
                return Icons.Material.Filled.PersonOff;

            return Icons.Material.Filled.Person;
        }

        protected Color GetAgentStatusColor(QueueAgentInfo agent)
        {
            if (agent.InCall)
                return Color.Warning;
            if (agent.Paused)
                return Color.Error;
            if (agent.Status == AsteriskDeviceStatus.InUse)
                return Color.Warning;
            if (agent.Status == AsteriskDeviceStatus.Unavailable)
                return Color.Default;

            return Color.Success;
        }

        protected string GetAgentCardClass(QueueAgentInfo agent)
        {
            if (agent.InCall)
                return "agent-incall";
            if (agent.Paused)
                return "agent-paused";
            if (agent.Status == AsteriskDeviceStatus.Unavailable)
                return "agent-unavailable";
            if (agent.Status == AsteriskDeviceStatus.InUse)
                return "agent-busy";

            return "agent-available";
        }

    }
}
