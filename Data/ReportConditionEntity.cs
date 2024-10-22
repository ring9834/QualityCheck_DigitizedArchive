using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj_FileManageNCheckApp
{
    [Serializable]
    public class ReportSearchConditionEntity
    {
        public ReportSearchConditionEntity() { }

        public ReportSearchConditionEntity(string fieldName, object value, string searchCondition)
        {
            this.FieldName = fieldName;
            this.Value = value;
            this.SearchCondition = searchCondition;
        }

        public string SearchCondition { get; set; }
        
        public string FieldName { get; set; }

        public object Value { get; set; }
        public override string ToString()
        {
            return this.FieldName.ToString();
        }
    }

    [Serializable]
    public class StatisticsCondition 
    {
        public StatisticsCondition() { }

        public List<ReportSearchConditionEntity> ReportConditionList { get; set; }

        public DataTable SearchConditionTable { get; set; }//搜索条件DATATABLE

        public List<string> StatisticsFieldsList { get; set; }

        public List<string> GroupFieldsList { get; set; }
    }

    [Serializable]
    public class ReportParams
    {
        public ReportParams() { }

        public SerializableDictionary<string, StatisticsCondition> CollectionBoxList { get; set; }

        public bool LandScape { get; set; }

        public bool OnlyCount { get; set; }

        public string ContentFontSize { get; set; }

        public string TitleFontSize { get; set; }

        public string ColumnCount { get; set; }

        public string ReportTitle { get; set; }

        public bool ShowDateTime { get; set; }

        public bool ShowPageNumber { get; set; }

        public bool ShowBorder { get; set; }
    }
}
