using Microsoft.AspNetCore.Components;
using System;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public partial class EventsPanel : EventsPanelView
    {
        [Parameter]
        [EditorRequired]
        public Panel Panel { get; set; } = default!;

        [Parameter]
        public uint PageSize { get; set; } = 20;

        [CascadingParameter]
        public TextSearchControl? TextSearch { get; set; }

        public PaggingControl Pagging { get; }

        public FilteringControl Filtering { get; internal set; }

        protected bool ShowFilter => !Filtering.External;


        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            Pagging.SetPageSize(PageSize);

            if (TextSearch != null)
                Filtering = new FilteringControl(TextSearch);

            // Listening panel options changes
            Panel.OnChanged += OnPanelChanged;
        }

        protected void OnPanelChanged(Panel _)
            => ShouldRefresh();

        public EventsPanel()
        {
            Filtering = new FilteringControl();
            Pagging = new PaggingControl();
        }

        protected Exception? Exception { get; set; }             
    }
}
