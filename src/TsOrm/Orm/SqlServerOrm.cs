using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using TsOrm.Core;

namespace TsOrm.Orm
{
    public class SqlServerOrm<T> : BaseOrm<T>
    {
        public SqlServerOrm(SqlConnection connection)
        : base(connection)
        {
        }
        public SqlServerOrm(string connectionString)
            : base(new SqlConnection(connectionString))
        {
        }
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
            var cmd = this.GetDbCommand(currExec.Item1, currExec.Item2) as SqlCommand;
            var dt = this.FillData(cmd);
            this.ClearQuery();
            return dt;
        }
        /// <summary>
        ///  执行存储过程
        /// </summary>
        /// <returns></returns>
        public override DataTable ExcuteDbProcedure()
        {
            if (this.IsQueryEmpty())
            {
                throw new Exception("查询为空");
            }
            var currExec = this._query;
            var cmd = this.GetDbCommand(currExec.Item1, currExec.Item2) as SqlCommand;
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
            var cmd = this.GetDbCommand(currExec.Item1, currExec.Item2) as SqlCommand;
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            this.ClearQuery();
            return dr;
        }
        /// <summary>
        /// 填充DataTable
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private DataTable FillData(SqlCommand cmd)
        {
            var adt = new SqlDataAdapter(cmd);
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
            SqlCommand cmd;
            if (null != this.Translation)
            {
                cmd = new SqlCommand(sqlStr, this.Connection as SqlConnection, this.Translation as SqlTransaction);
            }
            else
            {
                cmd = new SqlCommand(sqlStr, this.Connection as SqlConnection);
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
            var parameter = new SqlParameter();
            parameter.ParameterName = "@" + parameterName;
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Boolean:
                    parameter.SqlDbType = SqlDbType.Bit; break;
                case TypeCode.Byte:
                    parameter.SqlDbType = SqlDbType.TinyInt; break;
                case TypeCode.Int16:
                    parameter.SqlDbType = SqlDbType.SmallInt; break;
                case TypeCode.Int32:
                    parameter.SqlDbType = SqlDbType.Int; break;
                case TypeCode.Int64:
                    parameter.SqlDbType = SqlDbType.BigInt; break;
                case TypeCode.Decimal:
                    parameter.SqlDbType = SqlDbType.Money; break;
                case TypeCode.Double:
                    parameter.SqlDbType = SqlDbType.Float; break;
                case TypeCode.Single:
                    parameter.SqlDbType = SqlDbType.Real; break;
                case TypeCode.DateTime:
                    parameter.SqlDbType = SqlDbType.DateTime; break;
                case TypeCode.Char:
                    parameter.SqlDbType = SqlDbType.VarChar; break;
                case TypeCode.String:
                    parameter.SqlDbType = SqlDbType.NVarChar; break;
                case TypeCode.Object:
                    parameter.SqlDbType = SqlDbType.Variant; break;
                default:
                    throw new Exception("数据库不支持此类型的数据");
            }
            parameter.Value = value;
            return parameter;
        }


    }
    public class SqlServerOrm : BaseOrm
    {
        public SqlServerOrm(SqlConnection connection)
            : base(connection)
        {
        }
        public SqlServerOrm(string connectionString)
            : base(new SqlConnection(connectionString))
        {
        }
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
        ///  执行存储过程
        /// </summary>
        /// <returns></returns>
        public override DataTable ExcuteDbProcedure()
        {
            if (this.IsQueryEmpty())
            {
                throw new Exception("查询为空");
            }
            var currExec = this._query;
            var cmd = this.GetDbCommand(currExec.Item1, currExec.Item2) as SqlCommand;
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
            var cmd = this.GetDbCommand(currExec.Item1, currExec.Item2) as SqlCommand;
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
            var cmd = this.GetDbCommand(currExec.Item1, currExec.Item2) as SqlCommand;
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            this.ClearQuery();
            return dr;
        }
        /// <summary>
        /// 填充DataTable
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private DataTable FillData(SqlCommand cmd)
        {
            var adt = new SqlDataAdapter(cmd);
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
            SqlCommand cmd;
            if (null != this.Translation)
            {
                cmd = new SqlCommand(sqlStr, this.Connection as SqlConnection, this.Translation as SqlTransaction);
            }
            else
            {
                cmd = new SqlCommand(sqlStr, this.Connection as SqlConnection);
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
            var parameter = new SqlParameter();
            parameter.ParameterName = "@" + parameterName;
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Boolean:
                    parameter.SqlDbType = SqlDbType.Bit; break;
                case TypeCode.Byte:
                    parameter.SqlDbType = SqlDbType.TinyInt; break;
                case TypeCode.Int16:
                    parameter.SqlDbType = SqlDbType.SmallInt; break;
                case TypeCode.Int32:
                    parameter.SqlDbType = SqlDbType.Int; break;
                case TypeCode.Int64:
                    parameter.SqlDbType = SqlDbType.BigInt; break;
                case TypeCode.Decimal:
                    parameter.SqlDbType = SqlDbType.Money; break;
                case TypeCode.Double:
                    parameter.SqlDbType = SqlDbType.Float; break;
                case TypeCode.Single:
                    parameter.SqlDbType = SqlDbType.Real; break;
                case TypeCode.DateTime:
                    parameter.SqlDbType = SqlDbType.DateTime; break;
                case TypeCode.Char:
                    parameter.SqlDbType = SqlDbType.VarChar; break;
                case TypeCode.String:
                    parameter.SqlDbType = SqlDbType.NVarChar; break;
                case TypeCode.Object:
                    parameter.SqlDbType = SqlDbType.Variant; break;
                default:
                    throw new Exception("数据库不支持此类型的数据");
            }
            parameter.Value = value;
            return parameter;
        }


    }
}
