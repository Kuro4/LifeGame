using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LifeGame.Controls
{
    public class CellControl:Control
    {
        #region Properties
        #region IsEditable
        [Description("このCellが編集可能かどうか"), Category("共通")]
        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IsEditable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof(bool), typeof(CellControl), new PropertyMetadata(false));
        #endregion
        #region IsAlive
        [Description("このCellが生きているかどうか"), Category("共通")]
        public bool IsAlive
        {
            get { return (bool)GetValue(IsAliveProperty); }
            set { SetValue(IsAliveProperty, value); }
        }
        // Using a DependencyProperty as the backing store for IsAlive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAliveProperty =
            DependencyProperty.Register("IsAlive", typeof(bool), typeof(CellControl), new PropertyMetadata(false, OnIsAliveChanged));
        #endregion
        #region AliveBrush
        [Description("このCellが生の時のブラシ"), Category("ブラシ")]
        public Brush AliveBrush
        {
            get { return (Brush)GetValue(AliveBrushProperty); }
            set { SetValue(AliveBrushProperty, value); }
        }
        // Using a DependencyProperty as the backing store for AliveBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AliveBrushProperty =
            DependencyProperty.Register("AliveBrush", typeof(Brush), typeof(CellControl), new PropertyMetadata(Brushes.LawnGreen));
        #endregion
        #region DeadBrush
        [Description("このCellが死の時のブラシ"), Category("ブラシ")]
        public Brush DeadBrush
        {
            get { return (Brush)GetValue(DeadBrushProperty); }
            set { SetValue(DeadBrushProperty, value); }
        }
        // Using a DependencyProperty as the backing store for DeadBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeadBrushProperty =
            DependencyProperty.Register("DeadBrush", typeof(Brush), typeof(CellControl), new PropertyMetadata(Brushes.Black));
        #endregion
        #region CurrentBrush
        [Description("現在のCellのブラシ(読取専用)"), Category("ブラシ")]
        private static readonly DependencyPropertyKey CurrentBrushPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "CurrentBrush",
                typeof(Brush),
                typeof(CellControl),
                new PropertyMetadata(null));
        public static readonly DependencyProperty CurrentBrushProperty = CurrentBrushPropertyKey.DependencyProperty;
        public Brush CurrentBrush
        {
            get { return (Brush)GetValue(CurrentBrushProperty); }
            private set { this.SetValue(CurrentBrushPropertyKey, value); }
        }
        #endregion
        #endregion
        #region Method
        static CellControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CellControl),
                new FrameworkPropertyMetadata(typeof(CellControl)));
        }
        /// <summary>
        /// 編集可能なら生死を反転させる
        /// </summary>
        public void ReversIsAlive()
        {
            if(this.IsEditable) this.IsAlive = !this.IsAlive;
            this.Focus();
        }
        #endregion
        #region Override
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.CurrentBrush = this.IsAlive ? this.AliveBrush : this.DeadBrush;
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            this.ReversIsAlive();
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Return) this.ReversIsAlive();
        }
        #endregion
        #region CallBack
        private static void OnIsAliveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (CellControl)d;
            if ((bool)e.NewValue) self.CurrentBrush = self.AliveBrush;
            else self.CurrentBrush = self.DeadBrush;
        }
        #endregion
    }
}
