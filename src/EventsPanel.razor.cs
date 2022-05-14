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

        public PaggingControl Pagging { get; }

        public FilteringControl? Filtering { get; internal set; }

        [CascadingParameter]
        public TextSearchControl? TextSearch { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            
            if(TextSearch != null)
                Filtering = new FilteringControl(TextSearch);
        }

        public EventsPanel()
        {
            Pagging = new PaggingControl();
            Pagging.SetPageSize(20);
        }

        protected Exception? Exception { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                Pagging.OnPaggingChanged += (_) => ShouldRefresh();

                if (Service.IsConfigured)
                {
                    Service.OnChanged += (_, _) => ShouldRefresh();
                    try
                    {
                        if (Service != null)
                            await Service.StartAsync(System.Threading.CancellationToken.None);

                        Exception = null;
                    }
                    catch (Exception ex)
                    {
                        Exception = ex;
                    }

                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        protected bool HasCards() => Service.Panel.Cards.Any();              
    }
}
