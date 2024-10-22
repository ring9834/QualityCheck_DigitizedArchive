using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj_FileManageNCheckApp
{
    public class AttachedCodeClass
    {
        public string CodeValue { get; set; }
        public string CodeName { get; set; }
        public string UniqueCode { get; set; }
        public AttachedCodeClass(){}

        public AttachedCodeClass(string codeName, string codeValue, string uniqueCode)
        {
            this.CodeName = codeName;
            this.CodeValue = codeValue;
            this.UniqueCode = uniqueCode;
        }

        public override string ToString()
        {
            return this.CodeName;
        }
    }
}
