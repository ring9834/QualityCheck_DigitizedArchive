using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj_FileManageNCheckApp
{
    public class SearchConditionEntity
    {
        public SearchConditionEntity(){}

        public SearchConditionEntity(object text, object value, int uniquecode)
        {
            this.Text = text;
            this.Value = value;
            this.UniqueCode = uniquecode;
        }

        private int _uniquecode;
        private object _text;
        private object _Value;

        /// <summary>
        /// 键值
        /// </summary>
        public int UniqueCode
        {
            get { return this._uniquecode; }
            set { this._uniquecode = value; }
        }
        
        /// <summary>
        /// 显示值
        /// </summary>
        public object Text
        {
            get { return this._text; }
            set { this._text = value; }
        }
        /// <summary>
        /// 对象值
        /// </summary>
        public object Value
        {
            get { return this._Value; }
            set { this._Value = value; }
        }
        
        public override string ToString()
        {
            return this.Text.ToString();
        }
    }
}
