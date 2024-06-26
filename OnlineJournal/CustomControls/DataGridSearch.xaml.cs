using OnlineJournal.Model;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace OnlineJournal.CustomControls
{
    public partial class DataGridSearch : UserControl
    {
        public static readonly DependencyProperty CollectionProperty;
        public static readonly DependencyProperty CanSearchProperty;
        public static readonly DependencyProperty FilterProperty;

        public object Collection
        {
            get => GetValue(CollectionProperty);
            set => SetValue(CollectionProperty, value);
        }

        public bool CanSearch
        {
            get => (bool)GetValue(CanSearchProperty);
            set => SetValue(CanSearchProperty, value);
        }

        public Func<object, string, bool> Filter
        {
            get => (Func<object, string, bool>)GetValue(FilterProperty);
            set => SetValue(FilterProperty, value);
        }

        static DataGridSearch()
        {
            CollectionProperty = DependencyProperty.Register("Collection", typeof(object), typeof(DataGridSearch));
            CanSearchProperty = DependencyProperty.Register("CanSearch", typeof(bool), typeof(DataGridSearch));
            FilterProperty = DependencyProperty.Register("Filter", typeof(Func<object, string, bool>), typeof(DataGridSearch));
        }

        public DataGridSearch()
        {
            InitializeComponent();
            searchButton.Click += OnSearchClick;

            var binding = new Binding("CanSearch")
            {
                Source = this,
                Mode = BindingMode.OneWay
            };

            searchButton.SetBinding(Button.IsEnabledProperty, binding);
        }

        private void OnSearchClick(object sender, RoutedEventArgs e)
        {
            if (Collection == null)
                return;

            CollectionViewSource.GetDefaultView(Collection).Refresh();

            if (searchText.Text.Length >= 3)
                CollectionViewSource.GetDefaultView(Collection).Filter = FilterObjects;
        }

        private bool FilterObjects(object obj)
        {
            return Filter(obj, searchText.Text);
        }
    }
}
