using System;

namespace EasyXnb
{
    public static class ParseUtils
    {
        public static bool ParseOrDefault(string value, bool defaultValue)
        {
            return bool.TryParse(value, out var r) ? r : defaultValue;
        }

        public static TEnum ParseOrDefault<TEnum>(string value, TEnum defaultValue) where TEnum : struct
        {
            return Enum.TryParse(value, true, out TEnum r) ? r : defaultValue;
        }
    }
}