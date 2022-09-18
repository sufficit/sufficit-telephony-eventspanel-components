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
    public partial class EventsPanel : EventsPanelView
    {
        [Inject]
        public EventsPanelService Service { get; internal set; } = default!;

        [Parameter]
        public Panel Panel { get; set; } = default!;

        [CascadingParameter]
        public TextSearchControl? TextSearch { get; set; }

        public PaggingControl Pagging { get; }

        public FilteringControl? Filtering { get; internal set; }


        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (Panel == null)
                Panel = Service.Panel;

            if (TextSearch != null)
                Filtering = new FilteringControl(TextSearch);
        }

        public EventsPanel()
        {
            Filtering = new FilteringControl();
            Pagging = new PaggingControl();
            Pagging.SetPageSize(20);
        }

        protected Exception? Exception { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                // Listening pagging changes
                Pagging.OnPaggingChanged += (_) => ShouldRefresh();

                // Listening panel options changes
                Panel.OnChanged += (_) => ShouldRefresh();

                if (Service.IsConfigured)
                {
                    Service.OnChanged += (_, _) => ShouldRefresh();
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        protected bool HasCards() => Panel.Cards.Any();              
    }
}
