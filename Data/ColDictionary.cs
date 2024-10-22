using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj_FileManageNCheckApp
{
    public class ColDictionary
    {
        public string ShowName { get; set; }
        public string ColName { get; set; }
        public bool ColNull { get; set; }
        public string MaxLength { get; set; }
        public string ColShowValue { get; set; } //显示的哪个辅助代码

        public ColDictionary(){}

        public ColDictionary(string showName, string colName, bool colNull, string maxLength)
        {
            this.ShowName = showName;
            this.ColName = colName;
            this.ColNull = colNull;
            this.MaxLength = maxLength;
        }

        public override string ToString()
        {
            return this.ShowName;
        }
    }
}
