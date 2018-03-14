using LifeGame.InteractionRequestEx;
using LifeGame.Models;
using LifeGame.Utils;
using Microsoft.Practices.ObjectBuilder2;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LifeGame.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Properties
        public CellManager CellManager { get; }
        public ReactiveProperty<int> RowCount { get; private set; } = new ReactiveProperty<int>();
        public ReactiveProperty<int> ColumnCount { get; private set; } = new ReactiveProperty<int>();
        public ReactiveProperty<int> RowColumnCount { get; private set; } = new ReactiveProperty<int>();
        public ReadOnlyReactiveProperty<int> Generation { get; private set; }
        public ReadOnlyReactiveProperty<int> AlivCellsCount { get; private set; }
        public ReadOnlyReactiveProperty<int> CellsCount { get; private set; }
        public ReactiveProperty<int> UpdateSpeed { get; private set; } = new ReactiveProperty<int>();
        public ReactiveProperty<int> ToNextGenerationCount { get; private set; } = new ReactiveProperty<int>(1);
        public ReactiveProperty<int> ToPreviousGenerationCount { get; private set; } = new ReactiveProperty<int>(1);
        public ReactiveProperty<int> AliveWeight { get; private set; } = new ReactiveProperty<int>();
        public ReactiveProperty<bool> IsEditable { get; private set; } = new ReactiveProperty<bool>(true);
        public ReactiveProperty<bool> IsGenerationChanging { get; private set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<string> SaveFileName { get; private set; } = new ReactiveProperty<string>("");
        public ReactiveProperty<string> CurrentDirectoryName { get; private set; } = new ReactiveProperty<string>();
        public ReactiveProperty<FileInfo> SelectionFile { get; private set; } = new ReactiveProperty<FileInfo>();
        public ReactiveProperty<SolidColorBrush> AliveCellBrush { get; private set; } = new ReactiveProperty<SolidColorBrush>();
        public ReactiveProperty<SolidColorBrush> DeadCellBrush { get; private set; } = new ReactiveProperty<SolidColorBrush>();
        public ReactiveProperty<SolidColorBrush> CellBorderBrush { get; private set; } = new ReactiveProperty<SolidColorBrush>();
        public ReactiveCommand Start { get; private set; } = new ReactiveCommand();
        public ReactiveCommand TraceBack { get; private set; } = new ReactiveCommand();
        public ReactiveCommand Stop { get; private set; } = new ReactiveCommand();
        public ReactiveCommand ToNextGeneration { get; private set; } = new ReactiveCommand();
        public ReactiveCommand ToFirstGeneration { get; private set; } = new ReactiveCommand();
        public ReactiveCommand ToPreviousGeneration { get; private set; } = new ReactiveCommand();
        public ReactiveCommand ToLastGeneration { get; private set; } = new ReactiveCommand();
        public ReactiveCommand ChangeCellsRowAndColumn { get; private set; } = new ReactiveCommand();
        public ReactiveCommand ChangeCells { get; private set; } = new ReactiveCommand();
        public ReactiveCommand Random { get; private set; } = new ReactiveCommand();
        public ReactiveCommand Reset { get; private set; } = new ReactiveCommand();
        public ReactiveCommand DirectoryReload { get; private set; } = new ReactiveCommand();
        public ReactiveCommand Save { get; private set; } = new ReactiveCommand();
        public ReactiveCommand Load { get; private set; } = new ReactiveCommand();
        public ReactiveCommand ShowSettingsWindow { get; private set; } = new ReactiveCommand();
        public ReactiveCommand ChangeEditMode{ get; private set; } = new ReactiveCommand();

        public InteractionRequest<Notification> NotificationRequest { get; } = new InteractionRequest<Notification>();
        public InteractionRequest<Confirmation> ConfirmationRequest { get; } = new InteractionRequest<Confirmation>();
        public InteractionRequest<SettingsConfirmatinon> ShowSettingsWindowRequest { get; } = new InteractionRequest<SettingsConfirmatinon>();

        private ReactiveCollection<FileInfo> celleDataFiles = new ReactiveCollection<FileInfo>();
        public ReadOnlyReactiveCollection<FileInfo> CellDataFiles { get; private set; }
        #endregion
        #region Field
        private ReactiveTimer startTimer = new ReactiveTimer(TimeSpan.FromMilliseconds(100));
        private ReactiveTimer traceBackTimer = new ReactiveTimer(TimeSpan.FromMilliseconds(100));
        private int borderValue = 60;
        #endregion
        public MainWindowViewModel()
        {
            this.CellDataFiles = this.celleDataFiles.ToReadOnlyReactiveCollection();
            #region Settingsの読込
            this.RowCount.Value = SettingsHelper.RowCount;
            this.ColumnCount.Value = SettingsHelper.ColumnCount;
            this.RowColumnCount.Value = SettingsHelper.RowColumnCount;
            this.UpdateSpeed.Value = SettingsHelper.UpdateSpeed;
            this.AliveWeight.Value = SettingsHelper.AliveWeight;
            this.IsEditable.Value = SettingsHelper.IsEditable;
            this.CurrentDirectoryName.Value = SettingsHelper.CurrentDirectory;
            this.AliveCellBrush.Value = SettingsHelper.AliveCellBrush;
            this.DeadCellBrush.Value = SettingsHelper.DeadCellBrush;
            this.CellBorderBrush.Value = SettingsHelper.CellBorderColor;
            #endregion
            this.Reload();
#if DEBUG
            this.RowColumnCount.Value = 20;
#endif
            this.CellManager = new CellManager(this.RowColumnCount.Value);
            #region ReactivePropertyへ変換
            this.Generation = this.CellManager
                .ObserveProperty(x => x.Generation)
                .ToReadOnlyReactiveProperty();
            this.AlivCellsCount = this.CellManager
                .ObserveProperty(x => x.AliveCellsCount)
                .ToReadOnlyReactiveProperty();
            this.CellsCount = this.CellManager
                .ObserveProperty(x => x.CellsCount)
                .ToReadOnlyReactiveProperty();
            #endregion
            #region ReactiveCommandの実行条件
            this.Start = this.startTimer
                .ObserveProperty(x => x.IsEnabled)
                .Select(x => !x)
                .ToReactiveCommand();
            this.TraceBack = this.traceBackTimer
                .ObserveProperty(x => x.IsEnabled)
                .Select(x => !x)
                .CombineLatest(this.Generation, (x, y) => x && 1 < y)
                .ToReactiveCommand();
            this.ToNextGeneration = this.IsGenerationChanging
                .Select(x => !x)
                .ToReactiveCommand();
            this.ToLastGeneration = this.IsGenerationChanging
                .Select(x => !x)
                .ToReactiveCommand();
            this.ToPreviousGeneration = this.IsGenerationChanging
                .Select(x => !x)
                .CombineLatest(this.Generation,(x,y) => x && 1 < y)
                .ToReactiveCommand();
            this.ToFirstGeneration = this.IsGenerationChanging
                .Select(x => !x)
                .CombineLatest(this.Generation, (x, y) => x && 1 < y)
                .ToReactiveCommand();
            this.ChangeCellsRowAndColumn = this.CellManager
                .ObserveProperty(x => x.IsUpdating)
                .Select(x => !x)
                .ToReactiveCommand();
            this.ChangeCells = this.CellManager
                .ObserveProperty(x => x.IsUpdating)
                .Select(x => !x)
                .ToReactiveCommand();
            this.Random = this.IsGenerationChanging
                .Select(x => !x)
                .ToReactiveCommand();
            this.DirectoryReload = this.CurrentDirectoryName
                .Select(x => Directory.Exists(x))
                .ToReactiveCommand();
            this.Save = this.IsGenerationChanging
                .Select(x => !x)
                .CombineLatest(this.CurrentDirectoryName,(x,y) => x && Directory.Exists(y))
                .ToReactiveCommand();
            this.Load = this.IsGenerationChanging
                .Select(x => !x)
                .CombineLatest(this.SelectionFile,(x,y) =>
                {
                    if(x && y != null) return y.Exists;
                    return false;
                })
                .ToReactiveCommand();
            #endregion
            #region ReactiveProperty変更時の処理
            this.RowCount.Subscribe(x => SettingsHelper.RowCount = x);
            this.ColumnCount.Subscribe(x => SettingsHelper.ColumnCount = x);
            this.RowColumnCount.Subscribe(x => SettingsHelper.RowColumnCount = x);
            this.UpdateSpeed.Subscribe(x =>
            {
                this.startTimer.Interval = TimeSpan.FromMilliseconds(x);
                this.traceBackTimer.Interval = TimeSpan.FromMilliseconds(x);
                SettingsHelper.UpdateSpeed = x;
            });
            this.IsEditable.Subscribe(x => SettingsHelper.IsEditable = x);
            this.CurrentDirectoryName.Subscribe(x => SettingsHelper.CurrentDirectory = x);
            this.AliveCellBrush.Subscribe(x => SettingsHelper.AliveCellBrush = x);
            this.DeadCellBrush.Subscribe(x => SettingsHelper.DeadCellBrush = x);
            this.CellBorderBrush.Subscribe(x => SettingsHelper.CellBorderColor = x);
            #endregion
            #region ReactiveTimer進行時の処理内容
            this.startTimer.Subscribe(x =>
            {
                if (this.traceBackTimer.IsEnabled) this.traceBackTimer.Stop();
                this.CellManager.ToNextGeneration();
            });
            this.traceBackTimer.Subscribe(x =>
            {
                if (this.startTimer.IsEnabled) this.startTimer.Stop();
                if (1 < this.Generation.Value) this.CellManager.ToPreviousGeneration();
                else
                {
                    this.traceBackTimer.Stop();
                    this.IsGenerationChanging.Value = false;
                }
            });
            #endregion
            #region ReactiveCommandの実行内容
            this.Start.Subscribe(() =>
            {
                this.IsGenerationChanging.Value = true;
                if (this.CellManager.IsStarted) this.CellManager.RegisterState();
                this.startTimer.Reset();
                this.startTimer.Start();
            });
            this.TraceBack.Subscribe(() =>
            {
                this.IsGenerationChanging.Value = true;
                this.traceBackTimer.Reset();
                this.traceBackTimer.Start();
            });
            this.Stop.Subscribe(() =>
            {
                if (this.startTimer.IsEnabled) this.startTimer.Stop();
                else this.traceBackTimer.Stop();
                this.IsGenerationChanging.Value = false;
            });
            this.ToNextGeneration.Subscribe(() =>
            {
                if (this.CellManager.IsStarted) this.CellManager.RegisterState();
                this.CellManager.ToNextGeneration(this.ToNextGenerationCount.Value);
            });
            this.ToPreviousGeneration.Subscribe(() => this.CellManager.ToPreviousGeneration(this.ToPreviousGenerationCount.Value));
            this.ToFirstGeneration.Subscribe(() => this.CellManager.ToFirstGeneration());
            this.ToLastGeneration.Subscribe(() => this.CellManager.ToLastGeneration());
            this.Random.Subscribe(() => this.CellManager.Random(AliveWeight.Value));
            this.Reset.Subscribe(() =>
            {
                this.Stop.Execute();
                this.CellManager.Reset();
            });
            this.ChangeCellsRowAndColumn.Subscribe(() =>
            {
                if (this.borderValue <= this.RowColumnCount.Value)
                {
                    if (!this.ConfirmationRequest.RaiseEx("",Properties.Resources.Notice_TakeTime)) return;
                }
                this.CellManager.InitializeCells(this.RowColumnCount.Value);
            });
            this.ChangeCells.Subscribe(() =>
            {
                if (Math.Pow(this.borderValue, 2) <= this.RowCount.Value * this.ColumnCount.Value)
                {
                    if (!this.ConfirmationRequest.RaiseEx("", Properties.Resources.Notice_TakeTime)) return;
                }
                this.CellManager.InitializeCells(this.RowCount.Value, this.ColumnCount.Value);
            });
            this.ChangeEditMode.Subscribe(() => this.IsEditable.Value = !this.IsEditable.Value);
            this.DirectoryReload.Subscribe(() =>
            {
                this.Reload();
            });
            this.Save.Subscribe(() =>
            {
                if (this.SaveFileName.Value.Where(x => Properties.Resources.UnusableString.Contains(x)).Any())
                {
                    this.NotificationRequest.RaiseEx("", Properties.Resources.UnusableString + Environment.NewLine + Properties.Resources.Notice_UnusableString);
                    return;
                };
                var savePath = $@"{this.CurrentDirectoryName.Value}\{this.SaveFileName.Value}.csv";
                if (File.Exists(savePath))
                {
                    if (!this.ConfirmationRequest.RaiseEx("", Properties.Resources.Notice_OverwriteSave)) return;
                }
                if (!CellIO.SaveCellsData(this.CellManager, savePath))
                {
                    this.NotificationRequest.RaiseEx("", Properties.Resources.Notice_SaveFailed);
                }
                this.Reload();
            });
            this.Load.Subscribe(() =>
            {
                if (this.SelectionFile.Value == null) return;
                if (!CellIO.LoadCellsData(this.CellManager, this.SelectionFile.Value))
                {
                    this.NotificationRequest.RaiseEx("", Properties.Resources.Notice_LoadFailed);
                }
            });
            this.ShowSettingsWindow.Subscribe(() =>
            {
                SettingsHelper.SaveSettings();
                this.ShowSettingsWindowRequest.Raise(
                    new SettingsConfirmatinon
                    {
                        Title = Properties.Resources.Settings,
                        CurrentDirectory = SettingsHelper.CurrentDirectory,
                        AliveCellBrush = SettingsHelper.AliveCellBrush.Clone(),
                        DeadCellBrush = SettingsHelper.DeadCellBrush.Clone(),
                        CellBorderColor = SettingsHelper.CellBorderColor.Clone(),
                    },
                    n =>
                    {
                        if (n.Confirmed)
                        {
                            this.CurrentDirectoryName.Value = n.CurrentDirectory;
                            this.AliveCellBrush.Value = n.AliveCellBrush;
                            this.DeadCellBrush.Value = n.DeadCellBrush;
                            this.CellBorderBrush.Value = n.CellBorderColor;
                            SettingsHelper.SaveSettings();
                        }
                    });
            });
            #endregion
        }
        private void Reload()
        {
            if (Directory.Exists(this.CurrentDirectoryName.Value))
            {
                this.celleDataFiles.Clear();
                this.celleDataFiles.AddRange(Directory.GetFiles(this.CurrentDirectoryName.Value, "*.csv").Select(x => new FileInfo(x)));
            }
        }
    }
}