using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj_FileManageNCheckApp
{
    public class CatalogRecord
    {
        public string Table_Code { get; set; } //对应的表名，如案卷表，卷内表，还是件盒表

        public string Excel_File_Name { get; set; }//Excel文件名

        public string Import_Time { get; set; }//导入时间

        public string Import_User { get; set; }//导入者

        public string Record_Number { get; set; }//导入记录条数

        public string Config_Name { get; set; }//已有配置名称

        public string Config_XML { get; set; }//已有配置详细内容
    }
}
