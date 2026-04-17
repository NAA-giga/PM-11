using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace форма_Посетителя.Helpers
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;

            switch (status)
            {
                case "одобрена":
                    return Brushes.Green;
                case "не одобрена":
                    return Brushes.Red;
                case "проверка":
                    return Brushes.Orange;
                default:
                    return Brushes.Black;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
