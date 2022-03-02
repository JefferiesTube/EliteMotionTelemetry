using System;
using System.Text.RegularExpressions;
using System.Windows.Markup;

namespace EliteMotionTelemetry.Converter
{
    public abstract class ConverterBase : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        [System.Diagnostics.DebuggerStepThrough]
        public void AssertValueNotNull(object value)
        {
            if (value == null)
                throw new ArgumentNullException($"Converter {GetType().Name} does not accept null values");
        }

        [System.Diagnostics.DebuggerStepThrough]
        public void AssertParameterIsNull(object value)
        {
            if (value != null)
                throw new ArgumentNullException($"Converter {GetType().Name} does not accept any parameter");
        }

        [System.Diagnostics.DebuggerStepThrough]
        public void AssertParameterIsNotNull(object value)
        {
            if (value == null)
                throw new ArgumentNullException($"Converter {GetType().Name} requires a parameter");
        }

        [System.Diagnostics.DebuggerStepThrough]
        public void AssertParameterIsType<T>(object value)
        {
            if (!(value is T))
                throw new ArgumentException($"Converter {GetType().Name} requires parameter of type {typeof(T).Name} but received {value.GetType().Name}");
        }

        [System.Diagnostics.DebuggerStepThrough]
        public void AssertValueIsType<T>(object value)
        {
            if (!(value is T))
                throw new ArgumentException($"Converter {GetType().Name} requires value of type {typeof(T).Name} but received {value.GetType().Name}");
        }

        [System.Diagnostics.DebuggerStepThrough]
        public object[] ShowOneWayError()
        {
            throw new NotSupportedException($"{GetType().Name} only supports One-Way Bindings");
        }

        [System.Diagnostics.DebuggerStepThrough]
        public void AssertValueDerivesFrom<T>(object value)
        {
            if (!value.GetType().IsInstanceOfType(typeof(T)))
                throw new ArgumentException($"Converter {GetType().Name} requires the value to be assignable to {typeof(T).Name} but received {value.GetType().Name}");
        }

        [System.Diagnostics.DebuggerStepThrough]
        public void AssertParameterDerivesFrom<T>(object value)
        {
            if (!value.GetType().IsInstanceOfType(typeof(T)))
                throw new ArgumentException($"Converter {GetType().Name} requires the parameter to be assignable to {typeof(T).Name} but received {value.GetType().Name}");
        }

        [System.Diagnostics.DebuggerStepThrough]
        public void AssertParameterMatchesRegEx(object value, object parameter)
        {
            if (value is string s && parameter is string p)
            {
                if (!Regex.IsMatch(s, p))
                    throw new ArgumentException($"Converter {GetType().Name} failed the RegEx validation");
            }
            else
            {
                throw new ArgumentException($"Converter {GetType().Name} has a RegEx assertion that requires value and parameter to be strings, but the value is a {value?.GetType().Name} and the parameter is a {parameter?.GetType().Name}");
            }
        }
    }
}