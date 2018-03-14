using Microsoft.Practices;
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
    /// LifeGameのCellを表すクラス
    /// </summary>
    public class Cell:BindableBase
    {
        #region Properties
        private bool isAlive = false;
        /// <summary>
        /// このCellが生きているかどうか
        /// </summary>
        public bool IsAlive
        {
            get => this.isAlive;
            set => SetProperty(ref isAlive, value);
        }
        private Coordinate coordinate;
        /// <summary>
        ///このCellの座標
        /// </summary>
        public Coordinate Coordinate
        {
            get => coordinate;
            private set => SetProperty(ref coordinate, value);
        }
        private CellState state = CellState.Initial_Dead;
        /// <summary>
        /// 現在のCellの状態
        /// </summary>
        public CellState State
        {
            get => state;
            private set
            {
                //状態によって生死を決定する
                this.IsAlive = value.ToBool();
                SetProperty(ref state, value);
            }
        }
        private ObservableCollection<CellState> history = new ObservableCollection<CellState>();
        /// <summary>
        /// Cellの状態の履歴
        /// </summary>
        public ReadOnlyObservableCollection<CellState> History { get; }
        #endregion
        #region Field
        /// <summary>
        /// 周囲のセル
        /// </summary>
        private List<Cell> aroundCells;
        /// <summary>
        /// 次の世代の状態
        /// </summary>
        private CellState nextState;
        #endregion
        /// <summary>
        /// Coordinateを指定してCellを作成する
        /// </summary>
        /// <param name="coordinate">Coordinate(座標)</param>
        public Cell(Coordinate coordinate)
        {
            this.History = new ReadOnlyObservableCollection<CellState>(history);
            this.Coordinate = coordinate;
        }
        /// <summary>
        /// Coordinateと初期状態の生死を指定してCellを作成する
        /// </summary>
        /// <param name="coordinate">Coordinate(座標)</param>
        /// <param name="isAlive">Cellの生死</param>
        public Cell(Coordinate coordinate,bool isAlive)
        {
            this.History = new ReadOnlyObservableCollection<CellState>(history);
            this.Coordinate = coordinate;
            this.IsAlive = isAlive;
            this.history.Add(this.State);
        }
        /// <summary>
        /// 周囲のセルを取得する
        /// </summary>
        /// <param name="aroundCells"></param>
        public void SetAroundCells(IEnumerable<Cell> aroundCells)
        {
            //周囲のセル数が2個以下または9個異常ならエラー
            if (2 >= aroundCells.Count() || 9 <= aroundCells.Count()) throw new ArgumentOutOfRangeException($"セルの数が異常です\r\n{this.Coordinate.ToString()}の周囲のセル数:{aroundCells.Count()}");
            //this.aroundCells = aroundCells.ToList();
            this.aroundCells = aroundCells.ToList();
        }
        /// <summary>
        /// 現在の状態を記録する
        /// </summary>
        public void RegisterState()
        {
            if (this.IsAliveChanged())
            {
                this.State = this.IsAlive ? CellState.ChangedManual_Alive : CellState.ChangedManual_Dead;
            }
            this.history.Add(this.State);
        }
        /// <summary>
        /// 現在の状態を初期状態として記録する
        /// </summary>
        public void RegisterInitialState()
        {
            this.State = this.IsAlive ? CellState.Initial_Alive : CellState.Initial_Dead;
            this.history.Add(this.State);
        }
        /// <summary>
        /// 次の世代へ進める
        /// </summary>
        /// <param name="aroundCells">周囲のセル</param>
        public void ToNextGeneration()
        {
            this.State = this.nextState;
            this.history.Add(this.nextState);
        }
        /// <summary>
        /// 次の世代の状態を判定する
        /// </summary>
        public void DetermineStateNextGeneration()
        {
            var aliveCount = this.aroundCells.Where(x => x.IsAlive).Count();
            CellState res;
            if (!this.IsAlive)
            {
                //[誕生]
                if (aliveCount == 3) res = CellState.Birth;
                else res = CellState.Dead;
            }
            else
            {
                //[過疎]
                if (aliveCount <= 1) res = CellState.Depopulation;
                //[生存]
                else if (2 <= aliveCount && aliveCount <= 3) res = CellState.Survive;
                //[過密]
                else if (4 <= aliveCount) res = CellState.OverPopulation;
                else throw new Exception();
            }
            this.nextState = res;
        }
        /// <summary>
        /// 前の世代へ戻す
        /// </summary>
        public void ToPreviousGeneration()
        {
            if(this.history.Count > 1)
            {
                this.history.RemoveAt(this.history.Count - 1);
                this.State = this.history.Last();
            }
        }
        /// <summary>
        /// 現在の生死がルール外から変更されていないか確認する
        /// </summary>
        /// <returns></returns>
        public bool IsAliveChanged()
        {
            return this.history.Last().ToBool() != this.IsAlive;
        }
        /// <summary>
        /// 初期状態にリセットする
        /// </summary>
        public void Reset()
        {
            this.history.Clear();
            this.State = CellState.Initial_Dead;
        }
    }
}
