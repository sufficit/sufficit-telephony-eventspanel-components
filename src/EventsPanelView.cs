using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public abstract class EventsPanelView : ComponentBase, IDisposable
    {
        [CascadingParameter]
        public EventsPanelViewRefresh? ViewRefreshComponent { get; set; }

        protected bool ShouldRefreshView { get; set; }

        public bool IsRendered { get; internal set; }

        public void ShouldRefresh()
        {
            ShouldRefreshView = true;
            if (ViewRefreshComponent != null && !ViewRefreshComponent.IsSynchronous)
                ViewRefresh();
        }

        protected async void ViewRefresh()
        {
            if (ShouldRefreshView && IsRendered)
            {
                ShouldRefreshView = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        private bool _disposed;
        void IDisposable.Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                IsRendered = false;
                Dispose(true);
            }
        }

        public virtual void Dispose(bool disposing) { }

        protected override void OnAfterRender(bool firstRender)
        {
            IsRendered = true;
            if (firstRender)
            {
                // Applying sincronous refresh
                if(ViewRefreshComponent != null)
                    ViewRefreshComponent.Refresh += ViewRefresh;
            }
        }

        ~EventsPanelView() 
        { 
            if (ViewRefreshComponent != null) 
                ViewRefreshComponent.Refresh -= ViewRefresh; 
        }
    }
}
