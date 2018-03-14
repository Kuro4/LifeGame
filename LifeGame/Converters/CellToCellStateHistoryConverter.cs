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
    public class CellToCellStateHistoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var cell = (Cell)value;
            IEnumerable<string> res;
            switch (CultureInfo.CurrentCulture.Name)
            {
                case "en-US":
                    res = cell.History.Select((x, i) => $"{i + 1}:{x.ToEnglish()}");
                    break;
                case "ja-JP":
                    res = cell.History.Select((x, i) => $"{i + 1}:{x.ToJapanese()}");
                    break;
                default:
                    res = cell.History.Select((x, i) => $"Unsupported");
                    break;
            }
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
