using Box2dNet.Interop;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Atlantis.Game
{
    public class MotionLocksConverter : TypeConverter
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
                bool x = false;
                bool y = false;
                bool z = false;

                foreach (var ch in s.Split())
                {
                    if (ch.Length != 0)
                    {
                        if (ch.Equals("X", StringComparison.InvariantCultureIgnoreCase))
                        {
                            x = true;
                        }
                        else if (ch.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
                        {
                            y = true;
                        }
                        else if (ch.Equals("Z", StringComparison.InvariantCultureIgnoreCase))
                        {
                            z = true;
                        }
                        else
                        {
                            throw new FormatException($"Cannot convert {s} unexpected char: '{ch}'");
                        }
                    }
                }

                return new b2MotionLocks(x, y, z);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (value is b2MotionLocks v)
            {
                return $"{(v.linearX ? 'X' : "")}{(v.linearY ? 'Y' : "")}{(v.angularZ ? 'Z' : "")}";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
