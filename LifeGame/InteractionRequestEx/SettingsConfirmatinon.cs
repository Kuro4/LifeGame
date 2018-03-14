using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LifeGame.InteractionRequestEx
{
    public class SettingsConfirmatinon : Confirmation
    {
        public string CurrentDirectory { get; set; }
        public SolidColorBrush AliveCellBrush { get; set; }
        public SolidColorBrush DeadCellBrush { get; set; }
        public SolidColorBrush CellBorderColor { get; set; }
    }
}
