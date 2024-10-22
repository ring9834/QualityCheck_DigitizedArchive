using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj_FileManageNCheckApp
{
    public class ParametersForChildForms
    {
        public ParametersForChildForms() { }

        public string FileCodeName{ get; set; }//某档案类型库对应的表明

        public string ContentFormat { get; set; }//原文

        public ReportParams RP { get; set; }//统计报表条件
    }
}
