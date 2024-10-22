using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DotNet.DbUtilities;
using DevExpress.XtraEditors.Controls;

namespace Prj_FileManageNCheckApp
{
    public partial class XtraSuperSearch : DevExpress.XtraEditors.XtraUserControl, ICloneable 
    {
        object ICloneable.Clone()
        {
            return this.Clone();
        }
        public XtraSuperSearch Clone()
        {
            return (XtraSuperSearch)this.MemberwiseClone();
        } 

        public string AndOrValue
        {
            get
            {
                return comboBoxEdit1.Text;
            }
        }

        public ComboBoxItemCollection AndOrItems
        {
            get
            {
                return comboBoxEdit1.Properties.Items;
            }
        }

        public string FilledValue
        {
            get
            {
                return comboBoxEdit2.Text;
            }
        }

        public string ConditionValue
        {
            get
            {
                return comboBoxEdit3.Text;
            }
        }

        public ComboBoxItemCollection ConditionItems
        {
            get
            {
                return comboBoxEdit3.Properties.Items;
            }
        }

        public bool AndOrControlVisible
        {
            get
            {
                return comboBoxEdit1.Visible;
            }
            set { comboBoxEdit1.Visible = value; }
        }

        public XtraSuperSearch(bool initBool)
        {
            InitializeComponent();
            LoadComboboxItems();
            comboBoxEdit1.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            comboBoxEdit3.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            flowLayoutPanel1.AutoSize = true;
            flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        }

        public XtraSuperSearch()
        {
            InitializeComponent();
            //LoadComboboxItems();
            comboBoxEdit1.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            comboBoxEdit3.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            flowLayoutPanel1.AutoSize = true;
            flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        }

        void LoadComboboxItems ()
        {
            string sql = "SELECT Unique_code,code_name,code_value FROM t_config_codes WHERE parent_code IN(SELECT Unique_code FROM t_config_codes_base WHERE code_key='jstj')";
            DataTable dtcondtion = new DbHelper().Fill(sql);
            for (int i = 0; i < dtcondtion.Rows.Count; i++)
            {
                string name = dtcondtion.Rows[i]["code_name"].ToString();
                string value = dtcondtion.Rows[i]["code_value"].ToString();
                int uniquecode = int.Parse(dtcondtion.Rows[i]["Unique_code"].ToString());
                ComboBoxItem combItem = new ComboBoxItem(new SearchConditionEntity(name, value, uniquecode));
                comboBoxEdit3.Properties.Items.Add(combItem);
            }

            sql = "SELECT Unique_code,code_name,code_value FROM t_config_codes WHERE parent_code IN(SELECT Unique_code FROM t_config_codes_base WHERE code_key='andorcode')";
            dtcondtion = new DbHelper().Fill(sql);
            for (int i = 0; i < dtcondtion.Rows.Count; i++)
            {
                string name = dtcondtion.Rows[i]["code_name"].ToString();
                string value = dtcondtion.Rows[i]["code_value"].ToString();
                int uniquecode = int.Parse(dtcondtion.Rows[i]["Unique_code"].ToString());
                ComboBoxItem combItem = new ComboBoxItem(new SearchConditionEntity(name, value, uniquecode));
                comboBoxEdit1.Properties.Items.Add(combItem);
            }
            dtcondtion.Dispose();
        }

        //解决加载用户控件时闪烁的问题
        protected override CreateParams CreateParams
        {
            get
            {
                var parms = base.CreateParams;
                parms.Style &= ~0x02000000; // Turn off WS_CLIPCHILDREN 
                return parms;
            }
        }
    }
}
