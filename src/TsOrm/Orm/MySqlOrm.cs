using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using TsOrm.Core;

namespace TsOrm.Orm
{
    public class MySqlOrm<T> : BaseOrm<T>
    {
        public MySqlOrm(MySqlConnection connection)
        : base(connection)
        {

        }
        public MySqlOrm(string connectionString)
            : base(new MySqlConnection(connectionString))
        {

        }
        /// <summary>
        /// 执行返回第一行第一列
        /// </summary>
        /// <returns></returns>
        public override object ExecuteScalar()
        {
            if (this.IsQueryEmpty())
            {
                throw new Exception("查询为空");
            }
            var currExec = this._query;
            var result = this.GetDbCommand(currExec.Item1, currExec.Item2).ExecuteScalar();
            this.ClearQuery();
            return result;
        }
        /// <summary>
        /// 执行返回受影响行数
        /// </summary>
        /// <returns></returns>
        public override int ExecuteNonQuery()
        {
            if (this.IsQueryEmpty())
            {
                throw new Exception("查询为空");
            }
            var currExec = this._query;
            var result = this.GetDbCommand(currExec.Item1, currExec.Item2).ExecuteNonQuery();
            this.ClearQuery();
            return result;
        }
        /// <summary>
        /// 执行返回DataTable
        /// </summary>
        /// <returns></returns>
        public override DataTable ExcuteDataTable()
        {
            if (this.IsQueryEmpty())
            {
                throw new Exception("查询为空");
            }
            var currExec = this._query;
            var cmd = this.GetDbCommand(currExec.Item1, currExec.Item2) as MySqlCommand;
            var dt = this.FillData(cmd);
            this.ClearQuery();
            return dt;
        }
        /// <summary>
        /// 执行存储过程返回DataTable
        /// </summary>
        /// <returns></returns>
        public override DataTable ExcuteDbProcedure()
        {
            if (this.IsQueryEmpty())
            {
                throw new Exception("查询为空");
            }
            var currExec = this._query;
            var cmd = this.GetDbCommand(currExec.Item1, currExec.Item2) as MySqlCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            var dt = this.FillData(cmd);
            this.ClearQuery();
            return dt;
        }
        /// <summary>
        /// 返回DataReader
        /// </summary>
        /// <returns></returns>
        public override IDataReader ExcuteDataReaders()
        {
            if (this.IsQueryEmpty())
            {
                throw new Exception("查询为空");
            }
            var currExec = this._query;
            var cmd = this.GetDbCommand(currExec.Item1, currExec.Item2) as MySqlCommand;
            MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            this.ClearQuery();
            return dr;
        }
        /// <summary>
        /// 填充DataTable
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private DataTable FillData(MySqlCommand cmd)
        {
            var adt = new MySqlDataAdapter(cmd);
            var dt = new DataTable();
            adt.Fill(dt);
            adt.Dispose();
            return dt;
        }
        /// <summary>
        /// 获取Command
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override IDbCommand GetDbCommand(string sqlStr, DbParameter[] parameters)
        {
            MySqlCommand cmd;
            if (null != this.Translation)
            {
                cmd = new MySqlCommand(sqlStr, this.Connection as MySqlConnection, this.Translation as MySqlTransaction);
            }
            else
            {
                cmd = new MySqlCommand(sqlStr, this.Connection as MySqlConnection);
            }
            if (null != parameters && parameters.Any())
            {
                cmd.Parameters.AddRange(parameters);
            }
            return cmd;
        }
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override DbParameter GetDbParameter(string parameterName, object value)
        {
            var parameter = new MySqlParameter();
            parameter.ParameterName = "@" + parameterName;
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Boolean:
                    parameter.MySqlDbType = MySqlDbType.Bit; break;
                case TypeCode.Byte:
                    parameter.MySqlDbType = MySqlDbType.Byte; break;
                case TypeCode.Char:
                    parameter.MySqlDbType = MySqlDbType.VarChar; break;
                case TypeCode.DateTime:
                    parameter.MySqlDbType = MySqlDbType.DateTime; break;
                case TypeCode.Decimal:
                    parameter.MySqlDbType = MySqlDbType.Decimal; break;
                case TypeCode.Double:
                    parameter.MySqlDbType = MySqlDbType.Double; break;
                case TypeCode.Int16:
                    parameter.MySqlDbType = MySqlDbType.Int16; break;
                case TypeCode.Int32:
                    parameter.MySqlDbType = MySqlDbType.Int32; break;
                case TypeCode.Int64:
                    parameter.MySqlDbType = MySqlDbType.Int64; break;
                case TypeCode.Object:
                    parameter.MySqlDbType = MySqlDbType.Blob; break;
                case TypeCode.SByte:
                    parameter.MySqlDbType = MySqlDbType.UByte; break;
                case TypeCode.Single:
                    parameter.MySqlDbType = MySqlDbType.Float; break;
                case TypeCode.String:
                    parameter.MySqlDbType = MySqlDbType.VarChar; break;
                case TypeCode.UInt16:
                    parameter.MySqlDbType = MySqlDbType.UInt16; break;
                case TypeCode.UInt32:
                    parameter.MySqlDbType = MySqlDbType.UInt32; break;
                case TypeCode.UInt64:
                    parameter.MySqlDbType = MySqlDbType.UInt64; break;
                default:
                    throw new Exception("数据库不支持此类型的数据");
            }
            parameter.Value = value;
            return parameter;
        }

    }

    public class MySqlOrm : BaseOrm
    {
        public MySqlOrm(MySqlConnection connection)
        : base(connection)
        {
        }
        public MySqlOrm(string connectionString)
            : base(new MySqlConnection(connectionString))
        {
        }
        /// <summary>
        /// 执行返回第一行第一列
        /// </summary>
        /// <returns></returns>
        public override object ExecuteScalar()
        {
            if (this.IsQueryEmpty())
            {
                throw new Exception("查询为空");
            }
            var currExec = this._query;
            var result = this.GetDbCommand(currExec.Item1, currExec.Item2).ExecuteScalar();
            this.ClearQuery();
            return result;
        }
        /// <summary>
        /// 执行返回受影响行数数组
        /// </summary>
        /// <returns></returns>
        public override int ExecuteNonQuery()
        {
            if (this.IsQueryEmpty())
            {
                throw new Exception("查询为空");
            }
            var currExec = this._query;
            var result = this.GetDbCommand(currExec.Item1, currExec.Item2).ExecuteNonQuery();
            this.ClearQuery();
            return result;
        }
        /// <summary>
        /// 执行存储过程返回DataTable
        /// </summary>
        /// <returns></returns>
        public override DataTable ExcuteDbProcedure()
        {
            if (this.IsQueryEmpty())
            {
                throw new Exception("查询为空");
            }
            var currExec = this._query;
            var cmd = this.GetDbCommand(currExec.Item1, currExec.Item2) as MySqlCommand;
            cmd.CommandType = CommandType.StoredProcedure;
            var dt = this.FillData(cmd);
            this.ClearQuery();
            return dt;
        }
        /// <summary>
        /// 执行返回DataTable
        /// </summary>
        /// <returns></returns>
        public override DataTable ExcuteDataTable()
        {
            if (this.IsQueryEmpty())
            {
                throw new Exception("查询为空");
            }
            var currExec = this._query;
            var cmd = this.GetDbCommand(currExec.Item1, currExec.Item2) as MySqlCommand;
            var dt = this.FillData(cmd);
            this.ClearQuery();
            return dt;
        }
        /// <summary>
        /// 返回DataReader
        /// </summary>
        /// <returns></returns>
        public override IDataReader ExcuteDataReaders()
        {
            if (this.IsQueryEmpty())
            {
                throw new Exception("查询为空");
            }
            var currExec = this._query;
            var cmd = this.GetDbCommand(currExec.Item1, currExec.Item2) as MySqlCommand;
            MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            this.ClearQuery();
            return dr;
        }
        /// <summary>
        /// 填充DataTable
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private DataTable FillData(MySqlCommand cmd)
        {
            var adt = new MySqlDataAdapter(cmd);
            var dt = new DataTable();
            adt.Fill(dt);
            adt.Dispose();
            return dt;
        }
        /// <summary>
        /// 获取Command
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override IDbCommand GetDbCommand(string sqlStr, DbParameter[] parameters)
        {
            MySqlCommand cmd;
            if (null != this.Translation)
            {
                cmd = new MySqlCommand(sqlStr, this.Connection as MySqlConnection, this.Translation as MySqlTransaction);
            }
            else
            {
                cmd = new MySqlCommand(sqlStr, this.Connection as MySqlConnection);
            }
            if (null != parameters && parameters.Any())
            {
                cmd.Parameters.AddRange(parameters);
            }
            return cmd;
        }
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override DbParameter GetDbParameter(string parameterName, object value)
        {
            var parameter = new MySqlParameter();
            parameter.ParameterName = "@" + parameterName;
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Boolean:
                    parameter.MySqlDbType = MySqlDbType.Bit; break;
                case TypeCode.Byte:
                    parameter.MySqlDbType = MySqlDbType.Byte; break;
                case TypeCode.Char:
                    parameter.MySqlDbType = MySqlDbType.VarChar; break;
                case TypeCode.DateTime:
                    parameter.MySqlDbType = MySqlDbType.Datetime; break;
                case TypeCode.Decimal:
                    parameter.MySqlDbType = MySqlDbType.Decimal; break;
                case TypeCode.Double:
                    parameter.MySqlDbType = MySqlDbType.Double; break;
                case TypeCode.Int16:
                    parameter.MySqlDbType = MySqlDbType.Int16; break;
                case TypeCode.Int32:
                    parameter.MySqlDbType = MySqlDbType.Int32; break;
                case TypeCode.Int64:
                    parameter.MySqlDbType = MySqlDbType.Int64; break;
                case TypeCode.Object:
                    parameter.MySqlDbType = MySqlDbType.Blob; break;
                case TypeCode.SByte:
                    parameter.MySqlDbType = MySqlDbType.UByte; break;
                case TypeCode.Single:
                    parameter.MySqlDbType = MySqlDbType.Float; break;
                case TypeCode.String:
                    parameter.MySqlDbType = MySqlDbType.VarChar; break;
                case TypeCode.UInt16:
                    parameter.MySqlDbType = MySqlDbType.UInt16; break;
                case TypeCode.UInt32:
                    parameter.MySqlDbType = MySqlDbType.UInt32; break;
                case TypeCode.UInt64:
                    parameter.MySqlDbType = MySqlDbType.UInt64; break;
                default:
                    throw new Exception("数据库不支持此类型的数据");
            }
            parameter.Value = value;
            return parameter;
        }

    }
}
