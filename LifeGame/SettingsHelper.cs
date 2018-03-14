using LifeGame.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LifeGame
{
    /// <summary>
    /// Settingsのヘルパークラス
    /// </summary>
    public static class SettingsHelper
    {
        #region Properties
        public static int RowCount { get; set; }
        public static int ColumnCount { get; set; }
        public static int RowColumnCount { get; set; }
        public static int UpdateSpeed { get; set; }
        public static int AliveWeight { get; set; }
        public static bool IsEditable { get; set; }
        public static string CurrentDirectory { get; set; }
        public static SolidColorBrush AliveCellBrush { get; set; }
        public static SolidColorBrush DeadCellBrush { get; set; }
        public static SolidColorBrush CellBorderColor { get; set; }
        #endregion
        #region Field
        private static Settings settings = Settings.Default;
        #endregion
        static SettingsHelper()
        {
            LoadSettings();
            if (!settings.IsUpgraded)
            {
                settings.Upgrade();
                settings.IsUpgraded = true;
            }
        }
        /// <summary>
        /// 設定を読み込む
        /// </summary>
        public static void LoadSettings()
        {
            settings.Reload();
            //設定から読込
            RowCount = settings.RowCount;
            ColumnCount = settings.ColumnCount;
            RowColumnCount = settings.RowColumnCount;
            UpdateSpeed = settings.UpdateSpeed;
            AliveWeight = settings.AliveWeight;
            IsEditable = settings.IsEditable;
            CurrentDirectory = settings.CurrentDirectory;
            AliveCellBrush = settings.AliveCellBrush;
            DeadCellBrush = settings.DeadCellBrush;
            CellBorderColor = settings.CellBorderBrush;
        }
        /// <summary>
        /// 設定を保存する
        /// </summary>
        public static void SaveSettings()
        {
            //設定へ反映
            settings.RowCount = RowCount;
            settings.ColumnCount = ColumnCount;
            settings.RowColumnCount = RowColumnCount;
            settings.UpdateSpeed = UpdateSpeed;
            settings.AliveWeight = AliveWeight;
            settings.IsEditable = IsEditable;
            settings.CurrentDirectory = CurrentDirectory;
            settings.AliveCellBrush = AliveCellBrush;
            settings.DeadCellBrush = DeadCellBrush;
            settings.CellBorderBrush = CellBorderColor;

            settings.Save();
        }
        /// <summary>
        /// 設定を初期化する
        /// </summary>
        public static void Reset()
        {
            settings.Reset();
            LoadSettings();
        }
    }
}
