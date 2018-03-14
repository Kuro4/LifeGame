using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame.Models
{
    /// <summary>
    /// Cellの状態
    /// </summary>
    public enum CellState
    {
        /// <summary>
        /// 初期(生)
        /// </summary>
        Initial_Alive,
        /// <summary>
        /// 初期(死)
        /// </summary>
        Initial_Dead,
        /// <summary>
        /// 誕生
        /// </summary>
        Birth,
        /// <summary>
        /// 過疎
        /// </summary>
        Depopulation,
        /// <summary>
        /// 生存
        /// </summary>
        Survive,
        /// <summary>
        /// 過密
        /// </summary>
        OverPopulation,
        /// <summary>
        /// 死(変化無し)
        /// </summary>
        Dead,
        /// <summary>
        /// 手動変更(生)
        /// </summary>
        ChangedManual_Alive,
        /// <summary>
        /// 手動変更(死)
        /// </summary>
        ChangedManual_Dead,
    }

    public static class CellStateEx
    {
        /// <summary>
        /// CellStateを対応するBool値(IsAlive)に変換する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ToBool(this CellState value)
        {
            switch (value)
            {
                case CellState.Initial_Alive:
                    return true;
                case CellState.Initial_Dead:
                    return false;
                case CellState.Birth:
                    return true;
                case CellState.Depopulation:
                    return false;
                case CellState.Survive:
                    return true;
                case CellState.OverPopulation:
                    return false;
                case CellState.Dead:
                    return false;
                case CellState.ChangedManual_Alive:
                    return true;
                case CellState.ChangedManual_Dead:
                    return false;
                default:
                    return false;
            }
        }
        /// <summary>
        /// CellStateを対応するString(英語)に変換する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToEnglish(this CellState value)
        {
            switch (value)
            {
                case CellState.Initial_Alive:
                    return "Initial(Alive)";
                case CellState.Initial_Dead:
                    return "Initial(Dead)";
                case CellState.Birth:
                    return "Birth";
                case CellState.Depopulation:
                    return "Depopulation";
                case CellState.Survive:
                    return "Survive";
                case CellState.OverPopulation:
                    return "OverPopulation";
                case CellState.Dead:
                    return "Dead(Unchanged)";
                case CellState.ChangedManual_Alive:
                    return "ChangedManual(Alive)";
                case CellState.ChangedManual_Dead:
                    return "ChangedManual(Dead)";
                default:
                    return null;
            }
        }
        /// <summary>
        /// CellStateを対応するString(日本語)に変換する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToJapanese(this CellState value)
        {
            switch (value)
            {
                case CellState.Initial_Alive:
                    return "初期(生)";
                case CellState.Initial_Dead:
                    return "初期(死)";
                case CellState.Birth:
                    return "誕生";
                case CellState.Depopulation:
                    return "過疎";
                case CellState.Survive:
                    return "生存";
                case CellState.OverPopulation:
                    return "過密";
                case CellState.Dead:
                    return "死(変化無し)";
                case CellState.ChangedManual_Alive:
                    return "手動変更(生)";
                case CellState.ChangedManual_Dead:
                    return "手動変更(死)";
                default:
                    return null;
            }
        }
        /// <summary>
        /// Stringを対応するCellStateに変換する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static CellState ToCellState(this string value)
        {
            switch (value)
            {
                case "初期(生)":
                    return CellState.Initial_Alive;
                case "初期(死)":
                    return CellState.Initial_Dead;
                case "誕生":
                    return CellState.Birth;
                case "過疎":
                    return CellState.Depopulation;
                case "生存":
                    return CellState.Survive;
                case "過密":
                    return CellState.OverPopulation;
                case "死(変化無し)":
                    return CellState.Dead;
                case "手動変更(生)":
                    return CellState.ChangedManual_Alive;
                case "手動変更(死)":
                    return CellState.ChangedManual_Dead;
                default:
                    return CellState.Initial_Dead;
            }
        }
    }
}
