using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj_FileManageNCheckApp
{
    public class ComboboxItem
    {
        public ComboboxItem(string display,string value)
        {
            this.Display = display;
            this.Value = value;
        }
        public string Display { get; set; }

        public string Value { get; set; }

        public override string ToString()
        {
            return this.Display;
        }
    }
}
