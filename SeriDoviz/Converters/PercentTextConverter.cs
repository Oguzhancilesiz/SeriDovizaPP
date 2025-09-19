using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriDoviz.Converters
{
    // change zaten yüzde ise "N2%"; yoksa kibarca "—"
    public sealed class PercentTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return "—";
            if (value is IConvertible c)
            {
                var v = c.ToDouble(CultureInfo.InvariantCulture);
                return $"{v:N2}%";
            }
            return "—";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
