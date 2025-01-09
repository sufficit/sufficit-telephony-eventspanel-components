using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public class FilteringControl
    {
        private readonly TextSearchControl? _control;

        public FilteringControl() { }

        public FilteringControl(TextSearchControl? control)
        {
            _control = control;
            if (_control != null)
            {
                FilterText = _control.Value;
                _control.OnValueChanged += Filter;
            }
        }

        /// <summary>
        /// Child of an external control ?
        /// </summary>
        public bool External => _control != null;

        public string? FilterText { get; internal set; }

        public void Filter(ChangeEventArgs e) => Filter(e.Value?.ToString());

        public void Filter(string? text)
        {
            if (FilterText != text)
            {
                FilterText = text;
                OnFilterChanged?.Invoke(text);
            }
        }

        public event Action<string?>? OnFilterChanged;
    }
}
