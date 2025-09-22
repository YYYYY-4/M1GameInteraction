using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace Atlantis.Game
{
    public class Vector2Converter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext? context, [NotNullWhen(true)] Type? destinationType)
        {
            return base.CanConvertTo(context, destinationType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string s)
            {
                var values = s.Split(',');
                if (values.Length == 1)
                {
                    if (float.TryParse(values[0], out float x))
                    {
                        return new Vector2(x);
                    }
                }
                else if (values.Length == 2)
                {
                    if (float.TryParse(values[0], out float x) && float.TryParse(values[1], out float y))
                    {
                        return new Vector2(x, y);
                    }
                }
                throw new FormatException($"Cannot convert {s} to Vector2");
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (value is Vector2 v)
            {
                return $"{v.X},{v.Y}";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
