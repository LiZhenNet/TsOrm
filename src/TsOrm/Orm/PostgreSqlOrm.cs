using Npgsql;
using NpgsqlTypes;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using TsOrm.Core;

namespace TsOrm.Orm
{
    public class PostgreSqlOrm<T> : BaseOrm<T>
    {
        public PostgreSqlOrm(NpgsqlConnection connection)
        : base(connection)
        {
        }
        public PostgreSqlOrm(string connectionString)
            : base(new NpgsqlConnection(connectionString))
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
            var cmd = this.GetDbCommand(currExec.Item1, currExec.Item2) as NpgsqlCommand;
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
            var cmd = this.GetDbCommand(currExec.Item1, currExec.Item2) as NpgsqlCommand;
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
            var cmd = this.GetDbCommand(currExec.Item1, currExec.Item2) as NpgsqlCommand;
            NpgsqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            this.ClearQuery();
            return dr;
        }
        /// <summary>
        /// 填充DataTable
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private DataTable FillData(NpgsqlCommand cmd)
        {
            var adt = new NpgsqlDataAdapter(cmd);        
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
            NpgsqlCommand cmd;
            if (null != this.Translation)
            {
                cmd = new NpgsqlCommand(sqlStr, this.Connection as NpgsqlConnection, this.Translation as NpgsqlTransaction);
            }
            else
            {
                cmd = new NpgsqlCommand(sqlStr, this.Connection as NpgsqlConnection);
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
            var parameter = new NpgsqlParameter();
            parameter.ParameterName = "@" + parameterName;
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Boolean:
                    parameter.NpgsqlDbType = NpgsqlDbType.Boolean; break;
                case TypeCode.Byte:
                    parameter.NpgsqlDbType = NpgsqlDbType.Bytea; break;
                case TypeCode.Int64:
                    parameter.NpgsqlDbType = NpgsqlDbType.Bigint; break;
                case TypeCode.Int32:
                    parameter.NpgsqlDbType = NpgsqlDbType.Integer; break;
                case TypeCode.Int16:
                    parameter.NpgsqlDbType = NpgsqlDbType.Smallint; break;
                case TypeCode.Double:
                    parameter.NpgsqlDbType = NpgsqlDbType.Double; break;
                case TypeCode.Decimal:
                    parameter.NpgsqlDbType = NpgsqlDbType.Money; break;
                case TypeCode.Single:
                    parameter.NpgsqlDbType = NpgsqlDbType.Real; break;
                case TypeCode.String:
                    parameter.NpgsqlDbType = NpgsqlDbType.Text; break;
                case TypeCode.DateTime:
                    parameter.NpgsqlDbType = NpgsqlDbType.Date; break;
                default:
                    throw new Exception("数据库不支持此类型的数据");
            }
            parameter.Value = value;
            return parameter;
        }

    }

    public class PostgreSqlOrm : BaseOrm
    {
        public PostgreSqlOrm(NpgsqlConnection connection)
        : base(connection)
        {
        }
        public PostgreSqlOrm(string connectionString)
            : base(new NpgsqlConnection(connectionString))
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
            var cmd = this.GetDbCommand(currExec.Item1, currExec.Item2) as NpgsqlCommand;
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
            var cmd = this.GetDbCommand(currExec.Item1, currExec.Item2) as NpgsqlCommand;
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
            var cmd = this.GetDbCommand(currExec.Item1, currExec.Item2) as NpgsqlCommand;
            NpgsqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            this.ClearQuery();
            return dr;
        }
        /// <summary>
        /// 填充DataTable
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private DataTable FillData(NpgsqlCommand cmd)
        {
            var adt = new NpgsqlDataAdapter(cmd);
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
            NpgsqlCommand cmd;
            if (null != this.Translation)
            {
                cmd = new NpgsqlCommand(sqlStr, this.Connection as NpgsqlConnection, this.Translation as NpgsqlTransaction);
            }
            else
            {
                cmd = new NpgsqlCommand(sqlStr, this.Connection as NpgsqlConnection);
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
            var parameter = new NpgsqlParameter();
            parameter.ParameterName = "@" + parameterName;
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Boolean:
                    parameter.NpgsqlDbType = NpgsqlDbType.Boolean; break;
                case TypeCode.Byte:
                    parameter.NpgsqlDbType = NpgsqlDbType.Bytea; break;
                case TypeCode.Int64:
                    parameter.NpgsqlDbType = NpgsqlDbType.Bigint; break;
                case TypeCode.Int32:
                    parameter.NpgsqlDbType = NpgsqlDbType.Integer; break;
                case TypeCode.Int16:
                    parameter.NpgsqlDbType = NpgsqlDbType.Smallint; break;
                case TypeCode.Double:
                    parameter.NpgsqlDbType = NpgsqlDbType.Double; break;
                case TypeCode.Decimal:
                    parameter.NpgsqlDbType = NpgsqlDbType.Money; break;
                case TypeCode.Single:
                    parameter.NpgsqlDbType = NpgsqlDbType.Real; break;
                case TypeCode.String:
                    parameter.NpgsqlDbType = NpgsqlDbType.Text; break;
                case TypeCode.DateTime:
                    parameter.NpgsqlDbType = NpgsqlDbType.Date; break;
                default:
                    throw new Exception("数据库不支持此类型的数据");
            }
            parameter.Value = value;
            return parameter;
        }

    }
}
