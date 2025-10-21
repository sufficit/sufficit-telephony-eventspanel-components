using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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
            //var name = Content.ConnectedLineName;
            if (!string.IsNullOrWhiteSpace(num))
                return $"{num}";
            return num ?? "Desconhecido";
        }

        protected bool IsCalling => Content.State is AsteriskChannelState.Up or AsteriskChannelState.Ringing or AsteriskChannelState.Ring or AsteriskChannelState.Dialing or AsteriskChannelState.Busy or AsteriskChannelState.DialingOffhook or AsteriskChannelState.OffHook or AsteriskChannelState.PreRing;
    }
}
