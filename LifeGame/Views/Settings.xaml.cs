using LifeGame.InteractionRequestEx;
using LifeGame.Utils;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Interactivity.InteractionRequest;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LifeGame.Views
{
    public partial class Settings : MetroWindow
    {
        public SettingsConfirmatinon Confirmation
        {
            get { return this.DataContext as SettingsConfirmatinon; }
            set { this.DataContext = value; }
        }

        public Settings()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Confirmation.Confirmed = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Confirmation.Confirmed = false;
            this.Close();
        }

        private void ColorChangeButton_Click(object sender,RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;
            var brush = button.Background as SolidColorBrush;
            if (brush == null) return;
            var colorDialog = new System.Windows.Forms.ColorDialog() { Color = brush.ToDrawingColor() };
            if(colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) brush.Color = colorDialog.Color.ToMediaColor();
        }

        private void DirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
                Multiselect = false,
            };
            if (System.IO.Directory.Exists(this.Confirmation.CurrentDirectory)) folderDialog.DefaultDirectory = this.Confirmation.CurrentDirectory;

            if(folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.DirectoryTextBox.Text = folderDialog.FileName;
            }
        }
    }
}
