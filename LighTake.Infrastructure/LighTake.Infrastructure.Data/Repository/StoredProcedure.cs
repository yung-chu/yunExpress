using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using LighTake.Infrastructure.Common;
using System.Data;

namespace LighTake.Infrastructure.Data
{
    public interface IStoredProcedure
    {
        string Name { get; set; }

        IDbCommand Command { get; set; }

        List<object> Outputs { get; set; }
    }

    public class StoredProcedure : IStoredProcedure
    {
        public StoredProcedure(string spName, IDbConnection connection)
            : this(spName, connection, null)
        {

        }

        public StoredProcedure(string spName, IDbConnection connection, List<DbParameter> parameters)
        {
            Check.Argument.IsNotNull(connection, "connection");

            Name = spName;
            Command = connection.CreateCommand();

            Command.CommandText = spName;
            Command.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
            {
                foreach (DbParameter p in parameters)
                {
                    Command.Parameters.Add(p);
                }
            }

            Outputs = new List<object>();
        }

        #region IStoredProcedure Members

        public IDbCommand Command { get; set; }

        public string Name { get; set; }

        public List<object> Outputs { get; set; }

        #endregion

        #region Execution

        /// <summary>
        /// 设置存储过程参数
        /// </summary>
        /// <param name="param">参数</param>
        public void SetParameter(QueryParameter param)
        {
            IDbDataParameter p = Command.CreateParameter();
            p.ParameterName = param.ParameterName;
            p.Direction = param.Mode;
            p.DbType = param.DataType;

            if (p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.InputOutput)
                p.Size = param.Size;

            if (param.ParameterValue == null)
                p.Value = DBNull.Value;

            else if (param.DataType == DbType.Guid)
            {
                string paramValue = param.ParameterValue.ToString();
                if (!String.IsNullOrEmpty(paramValue))
                {
                    if (!paramValue.Equals("DEFAULT", StringComparison.InvariantCultureIgnoreCase))
                        p.Value = new Guid(param.ParameterValue.ToString());
                }
                else
                    p.Value = DBNull.Value;
            }
            else
                p.Value = param.ParameterValue;

            Command.Parameters.Add(p);
        }

        /// <summary>
        /// 执行存储过程不返回任何值
        /// </summary>
        public void Execute()
        {
            Command.ExecuteNonQuery();

            GetOutputParameters();
        }

        /// <summary>
        /// 执行存储过程返回指定类型数值
        /// </summary>
        /// <typeparam name="T">结果类型</typeparam>
        /// <returns>执行结果</returns>
        public T ExecuteScalar<T>()
        {
            T result = (T)Command.ExecuteScalar().ConvertTo<T>();

            GetOutputParameters();

            return result;
        }

        /// <summary>
        /// 执行存储过程返回指定类型列表
        /// </summary>
        /// <typeparam name="T">结果类型</typeparam>
        /// <returns>执行结果</returns>
        public List<T> ExecuteTypedList<T>() where T : new()
        {
            List<T> result =  Command.ExecuteReader().ToList<T>();

            GetOutputParameters();

            return result;
        }

        public void GetOutputParameters()
        {
            if (HasOutputParams())
            {
                foreach (IDbDataParameter p in Command.Parameters)
                {
                    if (p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.ReturnValue)
                    {
                        IDbDataParameter oVal = Command.Parameters[p.ParameterName] as IDbDataParameter;

                        if (oVal != null)
                        {
                            p.Value = oVal.Value;
                            Outputs.Add(p.Value);
                        }
                    }
                }
            }
        }

        public bool HasOutputParams()
        {
            bool bOut = false;

            foreach (IDbDataParameter param in Command.Parameters)
            {
                if (param.Direction != ParameterDirection.Input)
                {
                    bOut = true;
                    break;
                }
            }

            return bOut;
        }

        #endregion
    }

    public class QueryParameter
    {
        internal const ParameterDirection DefaultParameterDirection = ParameterDirection.Input;
        internal const int DefaultSize = 50;
        private ParameterDirection _mode = DefaultParameterDirection;

        private int _size = DefaultSize;
        public int Scale { get; set; }
        public int Precision { get; set; }

        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }

        /// <summary>
        /// 参数输入输出方向
        /// </summary>
        public ParameterDirection Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// 参数值
        /// </summary>
        public object ParameterValue { get; set; }

        /// <summary>
        /// 参数数据类型
        /// </summary>
        public DbType DataType { get; set; }
    }
}
