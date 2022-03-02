using System;
using System.Globalization;

namespace EliteMotionTelemetry.Converter.Visibility
{
    public class EnumToVisibilityConverter : ConverterMarkupExtension<EnumToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            AssertValueNotNull(value);
            AssertValueIsType<Enum>(value);
            AssertParameterIsNotNull(parameter);
            AssertParameterIsType<Enum>(parameter);

            Enum val = (Enum)value;
            return Equals(val, (Enum)parameter) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ShowOneWayError();
        }
    }
}