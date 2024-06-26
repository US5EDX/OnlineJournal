using System;
using System.Globalization;
using System.Windows.Data;

namespace OnlineJournal.Processings
{
    class InterprateRole : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int))
                return string.Empty;

            switch ((int)value)
            {
                case 1:
                    return "Викладач";
                case 2:
                    return "Аспірант";
                case 3:
                    return "Адміністратор";
                default:
                    return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string))
                return -1;


            switch (value as string)
            {
                case "Викладач":
                    return 1;
                case "Аспірант":
                    return 2;
                case "Адміністратор":
                    return 3;
                default:
                    return -1;
            }
        }
    }
}
