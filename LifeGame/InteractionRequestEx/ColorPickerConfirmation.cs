using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame.InteractionRequestEx
{
    public class ColorPickerConfirmation : Confirmation
    {
        public bool? AllowFullOpen { get; set; }
        public bool? AnyColor { get; set; }
        public Color? Color { get; set; }
        public int?[] CustomColors { get; set; }
        public bool? FullOpen { get; set; }
        public bool? ShowHelp { get; set; }
        public bool? SolidColorOnly { get; set; }
    }
}
