using Microsoft.Practices;
using Microsoft.Practices.ObjectBuilder2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame.Models
{
    /// <summary>
    /// LifeGameのCellの入出力を行うクラス
    /// </summary>
    public class CellIO
    {
        /// <summary>
        /// 指定したファイルパスのカンマ区切形式のファイルから指定したCellManagerへデータを読み込む
        /// </summary>
        /// <param name="cellManager">読込先のCellManager</param>
        /// <param name="filePath">読み込むファイルのパス</param>
        /// <returns>読み込みに成功したかどうか</returns>
        public static bool LoadCellsData(CellManager cellManager,string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                int columnCount = 0;
                int rowCount = 0;
                var readText = new List<bool>();
                try
                {
                    while (!reader.EndOfStream)
                    {
                        //1行を読み込んで,区切りで配列化
                        var text = reader.ReadLine().Split(',');
                        //1行目の配列の要素数を列数とする
                        if (columnCount < text.Count() && rowCount == 0) columnCount = text.Count();
                        //列数が一定でなければ中断
                        if (columnCount != text.Count()) return false;
                        rowCount++;
                        //文字列をboolに変換してreadTextに追加する(変換に失敗したらfalse)
                        readText.AddRange(text.Select(x => 
                        {
                            if (!bool.TryParse(x, out bool res)) res = false;
                            return res;
                        }));
                    }
                    //CellManagerへ読み込んだデータを入力
                    cellManager.InitializeCells(readText);
                    return true;
                }
                catch (Exception e) when ( e is IOException || e is OutOfMemoryException)
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// 指定したファイルパスのカンマ区切形式のファイルから指定したCellManagerへデータを読み込む
        /// </summary>
        /// <param name="cellManager">読み込み先のCellManager</param>
        /// <param name="fileInfo">読み込むファイル</param>
        /// <returns>読み込みに成功したかどうか</returns>
        public static bool LoadCellsData(CellManager cellManager,FileInfo fileInfo)
        {
            return LoadCellsData(cellManager, fileInfo.FullName);
        }
        /// <summary>
        /// 指定したファイルパスへCellManagerのデータをカンマ区切形式で保存する
        /// </summary>
        /// <param name="cellManager">保存するCellManager</param>
        /// <param name="filePath">書き込み先のファイルパス</param>
        /// <returns>保存に成功したかどうか</returns>
        public static bool SaveCellsData(CellManager cellManager,string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                string buf = "";
                try
                {
                    cellManager.Cells.Select((cell, index) => new { cell, index })
                        .ForEach(x =>
                        {

                            //Cellの生死を文字列にし、,を追加
                            buf += x.cell.IsAlive;
                            //1行分取得したら書き込む
                            if (x.index != 0 && (x.index + 1) % cellManager.ColumnCount == 0)
                            {
                                writer.WriteLine(buf);
                                buf = "";
                            }
                            else buf += ",";
                        });
                    return true;
                }
                catch (Exception e) when (e is ObjectDisposedException || e is IOException)
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// 指定したファイルパスへCellManagerのデータをカンマ区切形式で保存する
        /// </summary>
        /// <param name="cellManager">保存するCellManager</param>
        /// <param name="filePath">書き込み先のファイルパス</param>
        /// <returns>保存に成功したかどうか</returns>
        public static bool SaveCellsData(CellManager cellManager,FileInfo fileInfo)
        {
            return SaveCellsData(cellManager, fileInfo.FullName);
        }
    }
}
