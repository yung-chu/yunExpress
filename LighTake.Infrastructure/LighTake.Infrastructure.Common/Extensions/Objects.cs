using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace LighTake.Infrastructure.Common
{
    public static class Objects
    {
        /// <summary>
        /// 实现浅拷贝两对象相同名称的属性
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="targetEntity">目标对象</param>
        /// <param name="isNeedNullValue">连同NULL值属性也拷贝时，设置为true</param>
        public static void CopyTo(this object entity, object targetEntity, bool isNeedNullValue)
        {
            PropertyInfo[] entityProperties = entity.GetType().GetProperties();
            PropertyInfo[] targetEntityProperties = targetEntity.GetType().GetProperties();

            IDictionary<string, PropertyInfo> entityPropertiesNamePair = new Dictionary<string, PropertyInfo>();
            foreach (PropertyInfo pInfo in entityProperties)
            {
                if (!pInfo.CanRead) continue;
                entityPropertiesNamePair[pInfo.Name] = pInfo;
            }

            var targetPInfos = from targetPropertyInfo in targetEntityProperties
                               where entityPropertiesNamePair.Keys.Contains(targetPropertyInfo.Name)
                               select targetPropertyInfo;
            foreach (PropertyInfo targetPropertyInfo in targetPInfos)
            {
                if (!targetPropertyInfo.CanWrite) continue;
                object entityPInfoValue = entityPropertiesNamePair[targetPropertyInfo.Name].GetValue(entity, null);
                if ((isNeedNullValue) || (entityPInfoValue != null))
                {
                    targetPropertyInfo.SetValue(targetEntity, entityPInfoValue, null);
                }
            }
        }

        public static bool IsValueNullOrWhiteSpace(this object entity)
        {
            PropertyInfo[] entityProperties = entity.GetType().GetProperties();

            return entityProperties.Select(item => item.GetValue(entity, null)).All(val => val == null);
        }

        /// <summary>
        /// 实现浅拷贝两对象相同名称的属性，null值属性不拷贝
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="targetEntity"></param>
        public static void CopyTo(this object entity, object targetEntity)
        {
            CopyTo(entity, targetEntity, false);
        }

        /// <summary>
        /// 实现浅拷贝两对象相同名称的属性，null值属性不拷贝
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="targetEntity"></param>
        public static T Copy<T>(this object source)            
            where T : class ,new()
        {
            T tmpT = new T();
            CopyTo(source, tmpT);
            return tmpT;
        }

        /// <summary>
        /// 读取DataRow["Column"]的值,忽略掉DBNull
        /// </summary>
        public static string IgnoreDBNull(this object obj)
        {
            if (obj == System.DBNull.Value)
            {
                return string.Empty;
            }

            return obj.ToString();
        }

        /// <summary>
        /// 如果对象为Null则返回string.Empty
        /// </summary>
        public static string Null2StringEmpty(this object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            return obj.ToString();
        }

        /// <summary>
        /// 转换类型,支持可空类型
        /// </summary>
        public static object ConvertType(this object value, Type conversionType)
        {
            if (conversionType == null)
            {

                throw new ArgumentNullException("conversionType");

            }

            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;

                }

                System.ComponentModel.NullableConverter nullableConverter = new System.ComponentModel.NullableConverter(conversionType);

                conversionType = nullableConverter.UnderlyingType;
            }

            return Convert.ChangeType(value, conversionType);
        }


        public static object ConvertTo<T>(this object value)
        {
            Type conversionType = typeof(T);
            return ConvertTo(value, conversionType);
        }

        public static object ConvertTo(this object value, Type conversionType)
        {
            if (conversionType == null)
                throw new ArgumentNullException("conversionType");

            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                    return null;

                NullableConverter nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }
            else if (conversionType == typeof(Guid))
            {
                return new Guid(value.ToString());

            }
            else if (conversionType == typeof(Int64) && value.GetType() == typeof(int))
            {
                throw new InvalidOperationException("Can't convert an Int64 (long) to Int32(int). If you're using SQLite - this is probably due to your PK being an INTEGER, which is 64bit. You'll need to set your key to long.");
            }

            return Convert.ChangeType(value, conversionType);
        }


        /// <summary>
        /// 泛型转换类型,支持所有类型 
        /// </summary>
        public static T ConvertTo<T>(this IConvertible convertibleValue)
        {
            if (null == convertibleValue)
            {
                return default(T);
            }

            if (!typeof(T).IsGenericType)
            {
                return (T)Convert.ChangeType(convertibleValue, typeof(T));
            }
            else
            {
                Type genericTypeDefinition = typeof(T).GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof(Nullable<>))
                {
                    return (T)Convert.ChangeType(convertibleValue, Nullable.GetUnderlyingType(typeof(T)));
                }
            }
            throw new InvalidCastException(string.Format("Invalid cast from type \"{0}\" to type \"{1}\".",
                                                         convertibleValue.GetType().FullName, typeof(T).FullName));
        }

        public static T CastTo<T>(this object obj)
        {
            return (T)obj;
        }

    }
}
