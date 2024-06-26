using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OnlineJournal.Processings
{
    class CuratorVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string courseCode)
            {
                return courseCode != null ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
