using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Sufficit.Asterisk;
using Sufficit.Telephony.EventsPanel;
using System.Threading.Tasks;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public partial class CardChannelInfo
    {
        [EditorRequired]
        [Parameter]
        public ChannelInfoMonitor Monitor { get; set; } = default!;

        [CascadingParameter]
        public EventsPanelCardKind Kind { get; set; }

        [CascadingParameter(Name = "ClientCode")]
        public string? ClientCode { get; set; }

        protected ChannelInfo Content => Monitor.GetContent()!;

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (Monitor != null)
            {
                Monitor.OnChanged += ChannelOnChanged;
            }
        }

        private async void ChannelOnChanged(IMonitor? sender, object? state)
            => await InvokeAsync(StateHasChanged);

        protected string? Animation { get; set; }

        //protected string? GetLabelTooltip()
        //{
        //    if(Content.Hangup == null)
        //    {
        //        string label = Monitor.GetChannelLabel(Kind);
        //        return Content.Key != label ? Content.Key : null;
        //    } 
        //    else
        //    {
        //        return Content.Hangup?.Code + " :: " + Content.Hangup?.Description;
        //    }
        //}

        //protected string? GetExtraInfo()
        //{
        //    string? info;
        //    if (Monitor.IsInitiator)
        //    {
        //        info = Content.DirectInwardDialing;
        //        if(!string.IsNullOrWhiteSpace(info))
        //            info = "DID: " + info;
        //    } else
        //    {
        //        info = Content.OutboundCallerId;
        //        if (!string.IsNullOrWhiteSpace(info))
        //            info = "BINA: " + info;
        //    }
        //    return info;
        //}

        //protected string GetIconKey()
        //{
        //    Animation = string.Empty;
        //    if(Monitor != null)
        //    {
        //        switch (Content.State)
        //        {
        //            case AsteriskChannelState.Up: return EventsPanelDefaults.ICON_STATE_UP;
        //            case AsteriskChannelState.Ringing: return EventsPanelDefaults.ICON_STATE_RINGING;
        //            case AsteriskChannelState.Ring: return EventsPanelDefaults.ICON_STATE_RING;
        //            case AsteriskChannelState.Down: return EventsPanelDefaults.ICON_STATE_DOWN;
        //            case AsteriskChannelState.Dialing: return EventsPanelDefaults.ICON_STATE_DIALING;
        //            case AsteriskChannelState.Busy: return EventsPanelDefaults.ICON_STATE_BUSY;
        //            case AsteriskChannelState.Unknown: return EventsPanelDefaults.ICON_STATE_UNKNOWN;
        //        }
        //    }
        //    return "info";
        //}

        protected bool FromQueue => !string.IsNullOrWhiteSpace(Content.Queue);

        protected string Queue => "Fila de espera: " + Content.Queue;

        protected void OnChannelClick(MouseEventArgs e)
        {
            /*
            var alert = new SweetAlert()
            {
                TimerProgressBar = true,
                Timer = 5000,
                Title = "Ouvir Canal de Voz",
                Icon = "question",
                ShowDenyButton = true,
                DenyButtonText = "Não",
                ConfirmButtonText = "Continuar"
            };

            var Swal = UIService.SweetAlerts;
            var result = await Swal.Fire(alert);
            if (result != null)
            {
                if (result.IsConfirmed)
                {
                    Console.WriteLine("ouvindo canal de voz !");
                }
            }
            */
        }

        protected string GetDestinationInfo()
        {
            var num = Content.DialedExten ?? Content.ConnectedLineNum ?? Content.Exten;

            if (string.IsNullOrWhiteSpace(num))
                return "Desconhecido";

            // If this is a queue card and we have a client code, format the number
            if (Kind == EventsPanelCardKind.QUEUE && !string.IsNullOrWhiteSpace(ClientCode))
            {
                return FormatNumberWithClientCode(num);
            }

            return num;
        }

        /// <summary>
        /// Formats phone number by removing client code prefix if present (for queue cards)
        /// </summary>
        private string FormatNumberWithClientCode(string number)
        {
            if (string.IsNullOrWhiteSpace(number) || string.IsNullOrWhiteSpace(ClientCode))
                return number;

            // Check if the number starts with the client code
            if (number.StartsWith(ClientCode, StringComparison.OrdinalIgnoreCase))
            {
                // Remove the client code prefix
                return number.Substring(ClientCode.Length);
            }

            // Return original number if no match
            return number;
        }

        /// <summary>
        /// Gets the CSS class for the channel text based on call state
        /// </summary>
        protected string GetChannelTextClass()
        {
            var classes = new List<string>();

            // Add hangup class only when the call has actually ended
            if (Content?.Hangup != null)
                classes.Add("channel-hangup");

            // Add class to indicate channel is on hold
            if (Content?.OnHold == true)
                classes.Add("channel-onhold");

            // Add class to indicate the channel came from a queue (visual indicator only)


            return string.Join(" ", classes);
        }



        protected bool IsCalling => Content?.Hangup == null && Content.State is AsteriskChannelState.Up or AsteriskChannelState.Ringing or AsteriskChannelState.Ring or AsteriskChannelState.Dialing or AsteriskChannelState.Busy or AsteriskChannelState.DialingOffhook or AsteriskChannelState.OffHook or AsteriskChannelState.PreRing;

        /// <summary>
        /// Gets the icon color based on channel state
        /// </summary>
        protected MudBlazor.Color GetIconColor()
        {
            // Channel ended
            if (Content?.Hangup != null)
                return MudBlazor.Color.Default;

            // Channel on hold
            if (Content?.OnHold == true)
                return MudBlazor.Color.Warning;

            // Active call states
            if (Content?.State is AsteriskChannelState.Up)
                return MudBlazor.Color.Success;

            if (Content?.State is AsteriskChannelState.Ringing or AsteriskChannelState.Ring)
                return MudBlazor.Color.Info;

            if (Content?.State is AsteriskChannelState.Dialing or AsteriskChannelState.DialingOffhook)
                return MudBlazor.Color.Primary;

            if (Content?.State is AsteriskChannelState.Busy)
                return MudBlazor.Color.Error;

            // Default for other states
            return MudBlazor.Color.Default;
        }

        /// <summary>
        /// Gets the icon based on channel state
        /// </summary>
        protected string GetIcon()
        {
            if (Content?.Hangup != null)
                return Icons.Material.Filled.CallEnd;

            if (Content?.OnHold == true)
                return Icons.Material.Filled.Pause;

            if (Content?.State is AsteriskChannelState.Ringing or AsteriskChannelState.Ring)
                return Icons.Material.Filled.PhoneCallback;

            if (Content?.State is AsteriskChannelState.Dialing or AsteriskChannelState.DialingOffhook)
                return Icons.Material.Filled.PhoneForwarded;

            return Icons.Material.Filled.Call;
        }

        protected string GetCardChannelClass()
        {
            if (Kind == EventsPanelCardKind.QUEUE)
                return "px-2 py-1";

            return "px-1 pb-1";
        }
    }
}
