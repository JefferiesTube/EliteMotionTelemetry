using System;
using System.Globalization;
using System.Windows.Data;

namespace EliteMotionTelemetry.Converter
{
    public abstract class ConverterMarkupExtension<T> : ConverterBase, IValueConverter where T : class, new()
    {
        private static T _converter;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new T());
        }

        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
    }

    public abstract class MultiConverterMarkupExtension<T> : ConverterBase, IMultiValueConverter where T : class, new()
    {
        private static T _converter;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new T());
        }

        public abstract object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);

        public abstract object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture);
    }
}