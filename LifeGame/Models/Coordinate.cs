using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame.Models
{
    /// <summary>
    /// 座標
    /// </summary>
    public struct Coordinate
    {
        /// <summary>
        /// 行位置
        /// </summary>
        public int Row { get; }
        /// <summary>
        /// 列位置
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// 行,列の座標を指定してCoordinateを作成する
        /// </summary>
        /// <param name="row">行座標</param>
        /// <param name="column">列座標</param>
        public Coordinate(int row,int column)
        {
            this.Row = row;
            this.Column = column;
        }
        /// <summary>
        /// 指定したCoordinateがこの座標の周囲にあるかどうかを返す
        /// </summary>
        /// <param name="target">対象のCoordinate</param>
        /// <returns></returns>
        public bool IsAround(Coordinate target)
        {
            //この座標の周囲→行列が-1～+1かつ行列が同じでない
            return (Math.Abs(this.Row - target.Row) <= 1 && Math.Abs(this.Column - target.Column) <= 1) && !(this.Row == target.Row && this.Column == target.Column);
        }

        public override string ToString()
        {
            var res = "Unsupported";
            switch (CultureInfo.CurrentCulture.Name)
            {
                case "en-US":
                    res = $"Row:{this.Row},Column:{this.Column}";
                    break;
                case "ja-JP":
                    res = $"行:{this.Row},列:{this.Column}";
                    break;
                default:
                    break;
            }
            return res;
        }
    }
}
