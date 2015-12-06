using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using LighTake.Infrastructure.Common;

namespace LighTake.Infrastructure.Data
{
    public static class DatabaseExtensionscs
    {
        /// <summary>
        /// 将IDataReader对象转换成强类型List
        /// </summary>
        public static List<T> ToList<T>(this IDataReader rdr) where T : new()
        {
            List<T> result = new List<T>();
            Type iType = typeof(T);

            try
            {
                while (rdr.Read())
                {
                    T item = new T();
                    rdr.Load(item);
                    result.Add(item);
                }
            }
            finally
            {
                rdr.Close();
            }

            return result;
        }

        public static void Load<T>(this IDataReader rdr, T item)
        {
            Type iType = typeof(T);

            PropertyInfo[] cachedProps = iType.GetProperties();
            FieldInfo[] cachedFields = iType.GetFields();

            PropertyInfo currentProp;
            FieldInfo currentField = null;

            for (int i = 0; i < rdr.FieldCount; i++)
            {
                string pName = rdr.GetName(i);
                currentProp = cachedProps.SingleOrDefault(x => x.Name.Equals(pName, StringComparison.InvariantCultureIgnoreCase));

                //if the property is null, likely it's a Field
                if (currentProp == null)
                    currentField = cachedFields.SingleOrDefault(x => x.Name.Equals(pName, StringComparison.InvariantCultureIgnoreCase));

                if (currentProp != null && !DBNull.Value.Equals(rdr.GetValue(i)))
                {
                    Type valueType = rdr.GetValue(i).GetType();
                    if (valueType == typeof(SByte))
                        currentProp.SetValue(item, (Convert.ToSByte(rdr.GetValue(i).ToString())), null);
                    else if (currentProp.PropertyType == typeof(Guid))
                        currentProp.SetValue(item, rdr.GetGuid(i), null);
                    else
                        currentProp.SetValue(item, rdr.GetValue(i).ConvertTo(valueType), null);
                }
                else if (currentField != null && !DBNull.Value.Equals(rdr.GetValue(i)))
                {
                    Type valueType = rdr.GetValue(i).GetType();
                    if (valueType == typeof(SByte))
                        currentField.SetValue(item, (rdr.GetValue(i).ToString() == "1"));
                    else if (currentField.FieldType == typeof(Guid))
                        currentField.SetValue(item, rdr.GetGuid(i));
                    else
                        currentField.SetValue(item, rdr.GetValue(i).ConvertTo(valueType));
                }
            }
        }
    }
}
