using System;
using System.Collections.Generic;

namespace LighTake.Infrastructure.Common
{
    #region 用于简单返回两个字段实体，通常用作綁定下拉框
    /// <summary>
    /// 用于简单返回两个字段实体，通常用作綁定下拉框
    /// </summary>
    [Serializable]
    public class DataSourceBinder 
    {
        private string valueField;
        /// <summary>
        /// 用于值的字段
        /// </summary>
        public string ValueField
        {
            get { return valueField; }
            set { valueField = value; }
        }

        private string textField;
        /// <summary>
        /// 用于显示的字段
        /// </summary>
        public string TextField
        {
            get { return textField; }
            set { textField = value; }
        }
        
        private string textField_EN;
        /// <summary>
        /// 用于显示的字段(英文)
        /// </summary>
        public string TextField_EN
        {
            get { return textField_EN; }
            set { textField_EN = value; }
        }

        /// <summary>
        /// 生成实例
        /// </summary>
        public DataSourceBinder()
        {
        }
        /// <summary>
        /// 生成資料實例
        /// </summary>
        /// <param name="valuefield">用于值的文字</param>
        /// <param name="textfield">用于显示的文字</param>
        public DataSourceBinder(string valuefield, string textfield, string textfield_en)
        {
            valueField = valuefield;
            textField = textfield;
            textField_EN = textfield_en;
        }
        /// <summary>
        /// 生成資料實例
        /// </summary>
        /// <param name="valuefield">用于值的文字</param>
        /// <param name="textfield">用于显示的文字</param>
        public DataSourceBinder(string valuefield, string textfield)
        {
            valueField = valuefield;
            textField = textfield;
        }

        /*
        #region 绑定下拉框
        public static void DataBindDDL(IList<DataSourceBinder> lst, DropDownList ddlCtrl)
        {
            DataBindDDL(lst, ddlCtrl, "", true);
        }
        public static void DataBindDDL(IList<DataSourceBinder> lst, DropDownList ddlCtrl, string SelValue)
        {
            DataBindDDL(lst, ddlCtrl, SelValue, true);
        }
        public static void DataBindDDL(IList<DataSourceBinder> lst, DropDownList ddlCtrl, bool Flag)
        {
            DataBindDDL(lst, ddlCtrl, "", Flag);
        }
        public static void DataBindDDL(IList<DataSourceBinder> lst, DropDownList ddlCtrl, string SelValue, bool Flag)
        {
            string str;
            try
            {
                ddlCtrl.ClearSelection();
                ddlCtrl.DataSource = lst;
                ddlCtrl.DataTextField = "TextField";
                ddlCtrl.DataValueField = "ValueField";
                ddlCtrl.DataBind();
            }
            catch
            {
            }
            try
            {
                if (SelValue != "")
                {
                    ddlCtrl.Items.FindByValue(SelValue).Selected = true;
                }
            }
            catch
            {
            }
            if (Flag)
            {
                str = "";
                if (lst.Count != 0)
                {
                    str = "-- 请选择 --";
                }
                ddlCtrl.Items.Insert(0, new ListItem(str, ""));
            }
            else
            {
                str = "";
                if (lst.Count == 0)
                {
                    ddlCtrl.Items.Insert(0, new ListItem(str, ""));
                }
            }
        }
        #endregion
        */
    }

    public class DataBinding
    {
        public IList<DataSourceBinder> items = new List<DataSourceBinder>();

        public void SetDataBinder(string valuefield, string textfield, string textfieldEn)
        {

            DataSourceBinder item = new DataSourceBinder();
            item.ValueField = valuefield;
            item.TextField = textfield;
            item.TextField_EN = textfieldEn;
            items.Add(item);
        }

        public virtual string GetValue(int val, int lan)
        {
            string strResult = "";
            foreach (DataSourceBinder dbf in items)
            {
                if (dbf.ValueField == val.ToString())
                {
                    if (lan == 1)
                        strResult = dbf.TextField;
                    else
                        strResult = dbf.TextField_EN;
                }
            }
            return strResult;
        }
    }

    #endregion
}
