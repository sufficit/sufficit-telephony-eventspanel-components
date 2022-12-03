using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public abstract class EventsPanelView : ComponentBase
    {
        [CascadingParameter]
        public EventsPanelViewRefresh? ViewRefreshComponent { get; set; } = default!;

        protected bool ShouldRefreshView { get; set; }

        public void ShouldRefresh()
        {
            ShouldRefreshView = true;
            if (!ViewRefreshComponent.IsSincronous)
                ViewRefresh();
        }

        protected async void ViewRefresh()
        {
            if (ShouldRefreshView)
            {
                ShouldRefreshView = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                // Applying sincronous refresh
                ViewRefreshComponent.Refresh += ViewRefresh;
            }
        }

        ~EventsPanelView() { ViewRefreshComponent.Refresh -= ViewRefresh; }
    }
}
