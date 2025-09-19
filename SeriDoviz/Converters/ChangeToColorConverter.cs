using System;
using System.Globalization;
using Microsoft.Maui.Controls; // Application, Brush, SolidColorBrush, Color

namespace SeriDoviz.Converters
{
    public sealed class ChangeToColorConverter : IValueConverter
    {
        public Color UpColor { get; set; } = Colors.Green;
        public Color DownColor { get; set; } = Colors.Red;
        public Color NeutralColor { get; set; } = Colors.Gray;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 1) Sayıya çevir
            if (!TryToDouble(value, out var v))
            {
                // Arka plan istendiyse şeffaf fırça döndür, aksi halde nötr renk
                if (IsBg(parameter, targetType)) return new SolidColorBrush(Colors.Transparent);
                return NeutralColor;
            }

            // 2) Arka plan mı isteniyor? (Border.Background gibi)
            if (IsBg(parameter, targetType))
            {
                // App.xaml'da tanımladığın UpSoft/DownSoft renklerini al
                var upSoft = GetResourceColor("UpSoft", Colors.DarkGreen);
                var downSoft = GetResourceColor("DownSoft", Colors.DarkRed);
                return new SolidColorBrush(v >= 0 ? upSoft : downSoft);
            }

            // 3) Normal text rengi
            return v >= 0 ? UpColor : DownColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();

        private static bool TryToDouble(object value, out double v)
        {
            v = 0;
            if (value is null) return false;

            switch (value)
            {
                case string s when double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var d):
                    v = d; return true;
                case IConvertible c:
                    try { v = c.ToDouble(CultureInfo.InvariantCulture); return true; }
                    catch { return false; }
                default:
                    return false;
            }
        }

        private static bool IsBg(object parameter, Type targetType)
            => (parameter is string p && p.Equals("bg", StringComparison.OrdinalIgnoreCase))
               || targetType == typeof(Brush);

        private static Color GetResourceColor(string key, Color fallback)
        {
            if (Application.Current?.Resources.TryGetValue(key, out var obj) == true)
            {
                if (obj is Color c) return c;
                if (obj is SolidColorBrush b) return b.Color;
            }
            return fallback;
        }
    }
}
