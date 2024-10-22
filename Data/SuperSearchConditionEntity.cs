using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prj_FileManageNCheckApp
{
    public class SuperSearchConditionEntity
    {
        public SuperSearchConditionEntity(){}

        public SuperSearchConditionEntity(object id,object andOr, object condition, object value,object field)
        {
            this._id = id;
            this._andOr = andOr;
            this._condition = condition;
            this._value = value;
            this._field = field;
        }

        private object _id;
        private object _andOr;
        private object _condition;
        private object _value;
        private object _field;

        /// <summary>
        /// 键值
        /// </summary>
        public object ID
        {
            get { return this._id; }
            set { this._id = value; }
        }
        
        /// <summary>
        /// AND 或 OR
        /// </summary>
        public object AndOr
        {
            get { return this._andOr; }
            set { this._andOr = value; }
        }
        /// <summary>
        /// 条件符号 如，等于，大于...
        /// </summary>
        public object Condition
        {
            get { return this._condition; }
            set { this._condition = value; }
        }

        /// <summary>
        /// 值
        /// </summary>
        public object Value
        {
            get { return this._value; }
            set { this._value = value; }
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public object Field
        {
            get { return this._field; }
            set { this._field = value; }
        }
    }
}
