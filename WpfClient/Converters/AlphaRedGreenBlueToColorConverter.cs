using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfClient.Converters
{
    public class AlphaRedGreenBlueToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AlphaRedGreenBlue argb)
            {
                return Color.FromArgb(argb.Alpha, argb.Red, argb.Green, argb.Blue);
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                return new AlphaRedGreenBlue(color.A, color.R, color.G, color.B);
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
