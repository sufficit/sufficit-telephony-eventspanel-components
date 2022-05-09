using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Sufficit.Blazor.UI.Material;
using Sufficit.Telephony.EventsPanel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public partial class EventsPanelStats : EventsPanelView
    {
        [Inject]
        public EventsPanelService Service { get; internal set; } = default!;

        [Parameter]
        public FilteringControl? Filtering { get; set; }

        protected Exception? Exception { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                Service.OnChanged += Changed;
                Service.Channels.OnChanged += Changed;
            }
        }

        private void Changed(IMonitor? sender, object? state)
        {
            ShouldRefresh();
        }

        protected async Task Refresh()
        {
            if(Service != null && Service.IsConnected)
                await Service.GetPeerStatus();
        }

        protected int MaxButtons => Service?.Options?.MaxButtons ?? 0;

        protected void OnFilterChanged(ChangeEventArgs e) => Filtering?.Filter(e.Value?.ToString()!);
    }
}
