using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Sufficit.Telephony.EventsPanel.Components
{
    public class TextSearchControl
    {
        private bool disableByNavigation = false;

        /// <summary>
        /// Has any element to be filtered on this page, manualy update this 
        /// </summary>
        public bool CanSearch { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="active"></param>
        /// <returns>State Has Changed</returns>
        public bool Toggle(bool? active = default)
        {
            if (disableByNavigation)
            {
                if (CanSearch)
                {
                    CanSearch = !CanSearch;
                    OnChanged?.Invoke(CanSearch);

                    return true;
                }
            }
            else
            {
                if(active ?? false)
                {
                    CanSearch = !CanSearch;
                    OnChanged?.Invoke(CanSearch);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Text value to search
        /// </summary>
        public string? Value { get; internal set; }

        public void Update(string? text = default)
        {
            if (Value != text)
            {
                Value = text;
                OnValueChanged?.Invoke(Value);
            }
        }

        /// <summary>
        /// Text value changed
        /// </summary>
        public event Action<string?>? OnValueChanged;

        /// <summary>
        /// Toggled disabled / enabled
        /// </summary>
        public event Action<bool>? OnChanged;

        private readonly NavigationManager _navigation;

        public TextSearchControl(NavigationManager navigation)
        {
            _navigation = navigation;

            CanSearch = true;

            // setting initial value
            UpdateFromNavigation(_navigation);

            _navigation.LocationChanged += LocationChanged;
        }

        ~TextSearchControl() {
            _navigation.LocationChanged -= LocationChanged;
        }

        protected void UpdateFromNavigation(NavigationManager? navigation)
        {
            NavigationManager? current = navigation;
            if (current == null)
                current = _navigation;

            var uri = new Uri(current.Uri).Query;
            var query = HttpUtility.ParseQueryString(uri);
            var text = query["search"];

            disableByNavigation = !string.IsNullOrWhiteSpace(text);
            Toggle(disableByNavigation);

            if (disableByNavigation)
            {
                Update(text);
            } 
        }

        private void LocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
        {
            NavigationManager? current = _navigation;
            if (sender is NavigationManager navigation)
                current = navigation;

            UpdateFromNavigation(current);            
        }
    }
}
