using Microsoft.Practices;
using Microsoft.Practices.ObjectBuilder2;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame.Models
{
    /// <summary>
    /// LifeGameのCellを管理するクラス
    /// </summary>
    public class CellManager: BindableBase
    {
        #region Properties
        private ObservableCollection<Cell> cells = new ObservableCollection<Cell>();
        /// <summary>
        /// Cellのコレクション(読取専用)
        /// </summary>
        public ReadOnlyObservableCollection<Cell> Cells { get; }
        private int rowCount;
        /// <summary>
        /// 行数(3未満は3に強制)
        /// </summary>
        public int RowCount
        {
            get => this.rowCount;
            set => SetProperty(ref rowCount, value < 3 ? 3 : value);
        }
        private int columnCount;
        /// <summary>
        /// 列数(3未満は3に強制)
        /// </summary>
        public int ColumnCount
        {
            get => this.columnCount;
            set => SetProperty(ref this.columnCount, value < 3 ? 3 : value);
        }
        private int generation = 1;
        /// <summary>
        /// 現在の世代
        /// </summary>
        public int Generation
        {
            get => generation;
            private set
            {
                SetProperty(ref generation, value);
                this.AliveCellsCount = this.cells.Where(x => x.IsAlive).Count();
            }
        }
        private int lastGeneration = 1;
        /// <summary>
        /// 進めた最後の世代
        /// </summary>
        public int LastGeneration
        {
            get { return lastGeneration; }
            private set { SetProperty(ref lastGeneration, value); }
        }
        private int cellsCount;
        /// <summary>
        /// 現在のセル数
        /// </summary>
        public int CellsCount
        {
            get => cellsCount;
            private set => SetProperty(ref cellsCount, value);
        }
        private int aliveCellsCount;
        /// <summary>
        /// 現在の生存セル数
        /// </summary>
        public int AliveCellsCount
        {
            get => aliveCellsCount;
            private set => SetProperty(ref aliveCellsCount, value);
        }
        private bool isStarted = false;
        /// <summary>
        /// 現在のCellsで1度でもゲームを開始したかどうか
        /// </summary>
        public bool IsStarted
        {
            get { return isStarted; }
            private set { SetProperty(ref isStarted, value); }
        }
        private bool isUpdating = false;
        /// <summary>
        /// セルを更新中かどうか(セルを初期化中かどうか)
        /// </summary>
        public bool IsUpdating
        {
            get { return isUpdating; }
            private set { SetProperty(ref isUpdating, value); }
        }
        #endregion
        /// <summary>
        /// サイズを指定してCellManagerを作成する
        /// </summary>
        /// <param name="size">サイズ(行,列数)</param>
        public CellManager(int size)
        {
            this.Cells = new ReadOnlyObservableCollection<Cell>(this.cells);
            System.Windows.Data.BindingOperations.EnableCollectionSynchronization(this.Cells, new object());
            this.ColumnCount = size;
            this.RowCount = size;
            this.InitializeCells();
        }
        /// <summary>
        /// 行,列数を指定してCellManagerを作成する
        /// </summary>
        /// <param name="columnCount"></param>
        /// <param name="rowCount"></param>
        public CellManager(int columnCount,int rowCount)
        {
            this.Cells = new ReadOnlyObservableCollection<Cell>(this.cells);
            System.Windows.Data.BindingOperations.EnableCollectionSynchronization(this.Cells, new object());
            this.ColumnCount = columnCount;
            this.RowCount = rowCount;
            this.InitializeCells();
        }
        /// <summary>
        /// 現在の行,列数でCellsを初期化する
        /// Cellの生死のリストを指定するとその状態のセルを生成する
        /// </summary>
        /// <param name="cellsData">Cellの生死のリスト(省略可)</param>
        public void InitializeCells(IEnumerable<bool> cellsData = null)
        {
            this.CellsCount = this.RowCount * this.ColumnCount;
            this.cells.Clear();
            Enumerable.Range(0, this.RowCount)
                .ForEach(row => Enumerable.Range(0, this.ColumnCount)
                .ForEach(column =>
                {
                    if (cellsData == null) this.cells.Add(new Cell(new Coordinate(row + 1, column + 1)));
                    else this.cells.Add(new Cell(new Coordinate(row + 1, column + 1), cellsData.ElementAt((row * this.ColumnCount) + column)));
                }));
            this.cells.AsParallel().ForEach(x => x.SetAroundCells(this.GetAroundCells(x)));
            this.IsStarted = false;
        }
        /// <summary>
        /// 行,列数を指定してCellsを初期化する
        /// </summary>
        /// <param name="rowcolumnCount">行,列数</param>
        public void InitializeCells(int rowcolumnCount)
        {
            this.RowCount = rowcolumnCount;
            this.ColumnCount = rowcolumnCount;
            this.InitializeCells();
        }
        /// <summary>
        /// 行,列数を指定してCellsを初期化する
        /// </summary>
        /// <param name="rowCount">行数</param>
        /// <param name="columnCount">列数</param>
        public void InitializeCells(int rowCount,int columnCount)
        {
            this.RowCount = rowCount;
            this.ColumnCount = columnCount;
            this.InitializeCells();
        }
        /// <summary>
        /// 指定したCellの周囲のCellをIEnumerableで返す
        /// </summary>
        /// <param name="targetCell">対象のCell</param>
        /// <returns></returns>
        public IEnumerable<Cell> GetAroundCells(Cell targetCell)
        {
            foreach (var cell in this.cells)
            {
                if(targetCell.Coordinate.IsAround(cell.Coordinate)) yield return cell;
            }
        }
        /// <summary>
        /// 全てのセルを次の世代へ進める
        /// </summary>
        public void ToNextGeneration()
        {
            if (!this.IsStarted)
            {
                this.RegisterInitialState();
                this.IsStarted = true;
            }
            this.cells.AsParallel().ForEach(x => x.DetermineStateNextGeneration());
            this.cells.AsParallel().ForEach(x => x.ToNextGeneration());
            if (this.Generation == this.LastGeneration) this.LastGeneration++;
            this.Generation++;
        }
        /// <summary>
        /// 指定した数だけ全てのセルの世代を進める
        /// </summary>
        /// <param name="count"></param>
        public void ToNextGeneration(int count)
        {
            Enumerable.Range(0, count).ForEach(x => this.ToNextGeneration());
        }
        /// <summary>
        /// 進めた最後の世代まで進める
        /// </summary>
        public void ToLastGeneration()
        {
            if(this.Generation != this.LastGeneration) this.ToNextGeneration(this.LastGeneration - this.Generation);
        }
        /// <summary>
        /// 全てのセルを前の世代へ戻す
        /// </summary>
        public void ToPreviousGeneration()
        {
            if (!this.isStarted) return;
            this.cells.AsParallel().ForEach(x => x.ToPreviousGeneration());
            if(this.Generation > 1) this.Generation--;
        }
        /// <summary>
        /// 指定した数だけ全てのセルの世代を戻す
        /// </summary>
        /// <param name="count"></param>
        public void ToPreviousGeneration(int count)
        {
            Enumerable.Range(0, count).ForEach(x => this.ToPreviousGeneration());
        }
        /// <summary>
        /// 最初の世代に戻す
        /// </summary>
        public void ToFirstGeneration()
        {
            if (1 < this.Generation) this.ToPreviousGeneration(this.Generation - 1);
        }
        /// <summary>
        /// 現在のCell達の状態を記録する(1世代とみなす)
        /// </summary>
        public void RegisterState()
        {
            if (this.IsAliveChanged())
            {
                this.cells.ForEach(x => x.RegisterState());
                this.Generation++;
            }
        }
        /// <summary>
        /// 現在のCell達の状態を初期状態として記録する
        /// </summary>
        public void RegisterInitialState()
        {
            this.cells.ForEach(x => x.RegisterInitialState());
        }
        /// <summary>
        /// 現在のCell達の生死がルール外から変更されていないか確認する
        /// </summary>
        /// <returns></returns>
        public bool IsAliveChanged()
        {
            foreach (var cell in this.cells)
            {
                if (cell.IsAliveChanged()) return true;
            }
            return false;
        }
        /// <summary>
        /// 現在のセルの生死をランダムに変更する
        /// 引数を省略するとウェイトは50%となる
        /// </summary>
        /// <param name="aliveWeight">生である確率のウェイト(%)(省略可)</param>
        public void Random(int aliveWeight = 50)
        {
            this.Reset();
            var random = new Random();
            this.cells.AsParallel().ForEach(x => x.IsAlive = random.Next(1,101) <= aliveWeight);
            this.AliveCellsCount = this.cells.Where(x => x.IsAlive).Count();
        }
        /// <summary>
        /// 全てのセルをリセットする
        /// </summary>
        public void Reset()
        {
            this.cells.AsParallel().ForEach(x => x.Reset());
            this.Generation = 1;
            this.LastGeneration = 1;
            this.IsStarted = false;
        }
    }
}