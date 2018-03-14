using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame.Utils
{
    public static class ColorExtensions
    {
        #region Drawing To Media
        public static System.Windows.Media.Color ToMediaColor(this System.Drawing.Color drawingColor)
        {
            return System.Windows.Media.Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B);
        }
        #endregion
        #region Media To Drawing
        public static System.Drawing.Color ToDrawingColor(this System.Windows.Media.SolidColorBrush solidColorBrush)
        {
            return solidColorBrush.Color.ToDrawingColor();
        }
        public static System.Drawing.Color ToDrawingColor(this System.Windows.Media.Color mediaColor)
        {
            return System.Drawing.Color.FromArgb(mediaColor.A, mediaColor.R, mediaColor.G, mediaColor.B);
        }
        #endregion
    }
}
