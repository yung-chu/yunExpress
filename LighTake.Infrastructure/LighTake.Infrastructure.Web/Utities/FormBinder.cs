using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;

namespace LighTake.Infrastructure.Web.Utities
{
    /// <summary>
    /// 
    /// </summary>
    public class FormBinder
    {

        /// <summary>
        /// 绑定对象至控件（无前缀）
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="container"></param>
        public static void BindObjectToControls(object obj, Control container)
        {
            BindObjectToControls(obj, container, "");
        }

        /// <summary>
        /// 绑定对象至控件（带前缀）
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="container"></param>
        /// <param name="prefix"></param>
        public static void BindObjectToControls(object obj, Control container, string prefix)
        {
            if (obj == null) return;
            Type objType = obj.GetType();
            PropertyInfo[] objPropertiesArray = objType.GetProperties();
            foreach (PropertyInfo objProperty in objPropertiesArray)
            {
                Control control = container.FindControl(prefix + objProperty.Name);

                if (control != null)
                {
                    bool success = false;
                    bool isCustom = false;

                    if (!isCustom)
                    {
                        if (control is ListControl)
                        {
                            ListControl listControl = (ListControl)control;
                            string propertyValue = objProperty.GetValue(obj, null).ToString();
                            ListItem listItem = listControl.Items.FindByValue(propertyValue);
                            if (listItem != null) listItem.Selected = true;
                        }
                        else
                        {
                            Type controlType = control.GetType();
                            PropertyInfo[] controlPropertiesArray = controlType.GetProperties();
                            success = FindAndSetControlProperty(obj, objProperty, control, controlPropertiesArray, "Checked", typeof(bool));
                            if (!success)
                            {
                                success = FindAndSetControlProperty(obj, objProperty, control, controlPropertiesArray, "SelectedDate", typeof(DateTime));
                                if (success)
                                    FindAndSetControlProperty(obj, objProperty, control, controlPropertiesArray, "VisibleDate", typeof(DateTime));
                            }
                            if (!success)
                                success = FindAndSetControlProperty(obj, objProperty, control, controlPropertiesArray, "Value", typeof(String));
                            if (!success)
                                success = FindAndSetControlProperty(obj, objProperty, control, controlPropertiesArray, "Text", typeof(String));
                        }
                    }
                }
            }
        }
        private static bool FindAndSetControlProperty(object obj, PropertyInfo objProperty, Control control, PropertyInfo[] controlPropertiesArray, string propertyName, Type type)
        {
            foreach (PropertyInfo controlProperty in controlPropertiesArray)
            {
                if (controlProperty.Name == propertyName && controlProperty.PropertyType == type)
                {
                    controlProperty.SetValue(control, Convert.ChangeType(objProperty.GetValue(obj, null), type), null);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 绑定控件至对象（无前缀）
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="container"></param>
        public static void BindControlsToObject(object obj, Control container)
        {
            BindControlsToObject(obj, container, "");
        }

        /// <summary>
        /// 绑定控件至对象（带前缀）
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="container"></param>
        /// <param name="prefix"></param>
        public static void BindControlsToObject(object obj, Control container, string prefix)
        {
            if (obj == null) return;
            Type objType = obj.GetType();
            PropertyInfo[] objPropertiesArray = objType.GetProperties();
            foreach (PropertyInfo objProperty in objPropertiesArray)
            {
                bool success = false;
                bool isCustom = false;
                Control control = container.FindControl(prefix + objProperty.Name);

                if (control != null)
                {
                    if (!isCustom)
                    {
                        if (control is ListControl)
                        {
                            ListControl listControl = (ListControl)control;
                            if (listControl.SelectedItem != null)
                                objProperty.SetValue(obj, Convert.ChangeType(listControl.SelectedItem.Value, objProperty.PropertyType), null);
                        }
                        else
                        {
                            Type controlType = control.GetType();
                            PropertyInfo[] controlPropertiesArray = controlType.GetProperties();

                            success = FindAndGetControlProperty(obj, objProperty, control, controlPropertiesArray, "Checked", typeof(bool));
                            if (!success)
                                success = FindAndGetControlProperty(obj, objProperty, control, controlPropertiesArray, "SelectedDate", typeof(DateTime));
                            if (!success)
                                success = FindAndGetControlProperty(obj, objProperty, control, controlPropertiesArray, "Value", typeof(String));
                            if (!success)
                                success = FindAndGetControlProperty(obj, objProperty, control, controlPropertiesArray, "Text", typeof(String));
                        }
                    }
                }
            }
        }
        private static bool FindAndGetControlProperty(object obj, PropertyInfo objProperty, Control control, PropertyInfo[] controlPropertiesArray, string propertyName, Type type)
        {
            foreach (PropertyInfo controlProperty in controlPropertiesArray)
            {
                if (controlProperty.Name == propertyName && controlProperty.PropertyType == type)
                {
                    try
                    {
                        objProperty.SetValue(obj, Convert.ChangeType(controlProperty.GetValue(control, null), objProperty.PropertyType), null);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            return false;
        }
    }

}