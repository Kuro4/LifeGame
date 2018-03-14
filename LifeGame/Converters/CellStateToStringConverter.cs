using LifeGame.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LifeGame.Converters
{
    public class CellStateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var state = (CellState)value;
            var res = "";
            switch (CultureInfo.CurrentCulture.Name)
            {
                case "en-US":
                    res = state.ToEnglish();
                    break;
                case "ja-JP":
                    res = state.ToJapanese();
                    break;
                default:
                    res = "Unsupported";
                    break;
            }
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var str = (string)value;
            return str.ToCellState();
        }
    }
}
