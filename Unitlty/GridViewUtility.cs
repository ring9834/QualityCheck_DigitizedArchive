using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj_FileManageNCheckApp
{
    public static class GridViewUtility
    {
        /// <summary>
        /// 根据行，列索引来获取RepositoryItem
        /// </summary>
        /// <param name="view">GridView</param>
        /// <param name="rowIndex">行索引</param>
        /// <param name="columnIndex">列索引</param>
        /// <returns>RepositoryItem</returns>
        public static RepositoryItem GetRepositoryItem(this GridView view, int rowIndex, int columnIndex)
        {
            GridViewInfo _viewInfo = view.GetViewInfo() as GridViewInfo;
            GridDataRowInfo _viewRowInfo = _viewInfo.RowsInfo.FindRow(rowIndex) as GridDataRowInfo;
            return _viewRowInfo.Cells[columnIndex].Editor;
        }
    }
}
