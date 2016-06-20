using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using TsOrm.Context;
using TsOrm.Expression;
using TsOrm.Interface;

namespace TsOrm.Core
{
    public abstract class BaseOrm<T> : IOrm<T>
    {
        private IDbConnection _connection;
        protected BaseOrm(IDbConnection connection)
        {
            this.Connection = connection;
        }
        protected IDbConnection Connection
        {
            get
            {
                if (this._connection.State != ConnectionState.Open)
                {
                    this._connection.Open();
                }
                return this._connection;
            }
            private set
            {
                this._connection = value;
            }
        }
        /// <summary>
        ///  存储Sql 和 参数
        /// </summary>
        protected Tuple<string, DbParameter[]> _query = Tuple.Create<string, DbParameter[]>(null, null);
        /// <summary>
        /// 更新条件
        /// </summary>
        protected List<Tuple<string, object>> _updatecolume = new List<Tuple<string, object>>();
        /// <summary>
        /// Where条件
        /// </summary>
        protected string _wherecondition = null;
        public virtual BaseOrm<T> SqlQuery(string sqlstr)
        {
            return this.SqlQuery(sqlstr, null);
        }
        public virtual BaseOrm<T> SqlQuery(string sqlstr, params DbParameter[] parameters)
        {
            if (this.IsQueryEmpty())
            {
                throw new Exception("SqlQuery执行多次");
            }
            this._query = new Tuple<string, DbParameter[]>(sqlstr, parameters);
            return this;
        }
        public virtual BaseOrm<T> SqlQuery(string sqlstr, params object[] parameters)
        {
            if (!this.IsQueryEmpty())
            {
                throw new Exception("SqlQuery执行多次");
            }
            this._query = new Tuple<string, DbParameter[]>(sqlstr, GetDbParameters(parameters));
            return this;
        }
        /// <summary>
        /// 将Object 数组转换成Parameter 例：DbParameter(@0,value) 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private DbParameter[] GetDbParameters(params object[] parameters)
        {
            var count = parameters.Count();
            var dbParameters = new DbParameter[count];
            for (int i = 0; i < count; i++)
            {
                dbParameters[i] = this.GetDbParameter(i.ToString(), parameters[i]);
            }
            return dbParameters;
        }
        protected  virtual DbParameter GetDbParameter(string parameterName, object value)
        {
            throw new  NotImplementedException();
        }
        public virtual IDbCommand GetDbCommand(string sqlStr, DbParameter[] parameters)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 清空执行列表
        /// </summary>
        protected void ClearQuery()
        {   this._updatecolume = new List<Tuple<string, object>>();
            this._query = Tuple.Create<string, DbParameter[]>(null, null);
        }

        protected bool IsQueryEmpty()
        {
            if (_query == Tuple.Create<string, DbParameter[]>(null, null))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 添加更新列
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public BaseOrm<T> UpdateColumn(string columnname, object value)
        {
            this._updatecolume.Add(new Tuple<string, object>(columnname, value));
            return this;
        }
        public BaseOrm<T> UpdateColumn<TField>(Expression<Func<T, TField>> expr, object value)
        {
            ParserArgs a = new ParserArgs();
            Parser.Select(expr.Body, a);
            this._updatecolume.Add(new Tuple<string, object>(a.Builder.ToString(), value));
            return this;
        }
        public BaseOrm<T> Where(string wherecondition)
        {
            if (string.IsNullOrEmpty(wherecondition))
            {
                this._wherecondition = "";
            }
            else
            {
                this._wherecondition = " WHERE " + wherecondition;
            }
            return this;
        }
        public BaseOrm<T> Where(Expression<Func<T, bool>> expr)
        {
            StringBuilder where = new StringBuilder();
            ParserArgs a = new ParserArgs();
            Parser.Where(expr.Body, a);
            where.Append(" WHERE" + a.Builder);
            this._wherecondition = where.ToString();
            return this;
        }
        #region 增删改查
        public int InsertEntity(T model)
        {
            var key = new StringBuilder();
            var value = new StringBuilder();
            var dbParameterList = new List<DbParameter>();
            foreach (var currColumn in EntityContext<T>.Columns)
            {
                if (currColumn.Key && currColumn.Ignore)
                {
                    continue;
                }
                key.Append(string.Format("{0},", currColumn.ColumnName));
                value.Append(string.Format("@{0},", currColumn.ColumnName));
                dbParameterList.Add(this.GetDbParameter(currColumn.ColumnName, currColumn.GetterDelegate(model)));
            }
            if (key.Length > 0)
            {
                key.Remove(key.Length - 1, 1);
            }
            if (value.Length > 0)
            {
                value.Remove(value.Length - 1, 1);
            }
            return this.CommnEntityExecute(string.Format("INSERT INTO {0} ({1}) VALUES ({2})", EntityContext<T>.TableName, key, value), dbParameterList.ToArray());
        }
        public int Delete(string keyValue)
        {
            var key = (from c in EntityContext<T>.Columns where c.Key select c).FirstOrDefault();
            if (null == key)
            {
                throw new Exception("实体未设置主键");
            } 
            return this.CommnEntityExecute(string.Format("DELETE FROM {0} WHERE {1}", EntityContext<T>.TableName, string.Format("{0}=@{0}", key.ColumnName))
                , new DbParameter[] { this.GetDbParameter(key.ColumnName, keyValue) });
        }
        public int Delete(T model)
        {
            var key = (from c in EntityContext<T>.Columns where c.Key select c).FirstOrDefault();
            if (null == key)
            {
                throw new Exception("实体未设置主键");
            }
            return this.CommnEntityExecute(string.Format("DELETE FROM {0} WHERE {1}", EntityContext<T>.TableName, string.Format("{0}=@{0}", key.ColumnName)),
                new DbParameter[] { this.GetDbParameter(key.ColumnName, key.GetterDelegate(model)) });
        }
        public int UpdateEntity(T model)
        {
            var where = string.Empty;
            var valueStr = new StringBuilder();
            var dbparameterList = new List<DbParameter>();
            foreach (var currColumn in EntityContext<T>.Columns)
            {
                dbparameterList.Add(this.GetDbParameter(currColumn.ColumnName, currColumn.GetterDelegate(model)));
                if (currColumn.Key)
                {
                    where = string.Format("{0}=@{0}", currColumn.ColumnName);
                    continue;
                }
                if (currColumn.Ignore)
                {
                    continue;
                }
                valueStr.Append(string.Format("{0}=@{0},", currColumn.ColumnName));
            }
            if (!string.IsNullOrEmpty(_wherecondition))
            {
                where = _wherecondition;
            }
            if (string.IsNullOrWhiteSpace(where))
            {
                throw new Exception("实体模型未设置主键");
            }
            if (valueStr.Length > 0)
            {
                valueStr.Remove(valueStr.Length - 1, 1);
            } 
            return this.CommnEntityExecute(string.Format("UPDATE {0} SET {1} WHERE {2}", EntityContext<T>.TableName, valueStr, where), dbparameterList.ToArray());
        }
        public List<T> ToEntityList()
        {
            var data = this.ExcuteDataTable();
            List<T> entitylist = new List<T>();
            for (int i = 0; i < data.Rows.Count; i++)
            {
                entitylist.Add(DataRowConvert.ToEntity<T>(data.Rows[i]));
            }
            return entitylist;
        }
        public List<T> ToEntityList(CommandType cmdtype)
        {
            DataTable data = new DataTable();
            if (cmdtype == CommandType.StoredProcedure)
            {
                data = this.ExcuteDbProcedure();
                List<T> entitylist = new List<T>();
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    entitylist.Add(DataRowConvert.ToEntity<T>(data.Rows[i]));
                }
                return entitylist;
            }
            else if(cmdtype==CommandType.Text)
            {
                return this.ToEntityList();
            }
            return null;
        }
        public T ToEntity()
        {
            var dt = this.ExcuteDataTable();
            if (dt.Rows.Count == 0)
            {
                return default(T);
            }
            if (dt.Columns.Count == 1)
            {
                return (T)Convert.ChangeType(dt.Rows[0][0], typeof(T));
            }
            return DataRowConvert.ToEntity<T>(dt.Rows[0]);
        }
        /// <summary>
        /// 根据主键查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public T QueryEntityByKey(string keyValue) 
        {
            var key = (from c in EntityContext<T>.Columns where c.Key select c).FirstOrDefault();
            if (key == null)
            {
                throw new Exception("实体未设置主键");
            }
            this.SqlQuery(
                string.Format("DELETE FROM {0} WHERE {1}", EntityContext<T>.TableName,
                    string.Format("{0}=@{0}", key.ColumnName)),
                new DbParameter[] { this.GetDbParameter(key.ColumnName, keyValue) });
            var data = this.ExcuteDataTable();
            if (data.Rows.Count > 0)
            {
                return DataRowConvert.ToEntity<T>(data.Rows[0]);
            }
            else
            {
                return default(T);
            }
        }
        public int Update()
        {
            if (_wherecondition == null)
            {
                throw new Exception("更新列数不能为null,无Where条件调用UpdateWhere方法参数为空字符串");
            }
            if (_updatecolume.Count <= 0)
            {
                throw new Exception("更新列数不能为0");
            }
            var valueStr = new StringBuilder();
            var dbparameterList = new List<DbParameter>();
            List<string> columnnamelist = EntityContext<T>.Columns.Select(x => x.ColumnName).ToList();
            for (var i = 0; i < _updatecolume.Count; i++)
            {
                if (columnnamelist.Contains(_updatecolume[i].Item1))
                {
                    valueStr.Append(string.Format("{0}=@{0},", _updatecolume[i].Item1));
                    dbparameterList.Add(this.GetDbParameter(_updatecolume[i].Item1, _updatecolume[i].Item2));
                }
            }
            if (valueStr.Length > 0)
            {
                valueStr.Remove(valueStr.Length - 1, 1);
            }
            else
            {
                throw new Exception("匹配更新列数为0");
            }
            return this.CommnEntityExecute(string.Format("UPDATE {0} SET {1}   {2}", EntityContext<T>.TableName, valueStr, _wherecondition), dbparameterList.ToArray());
        }

        /// <summary>
        /// 执行Entity操作
        /// </summary>
        /// <param name="sqlstr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private int CommnEntityExecute(string sqlstr, DbParameter[] parameters)
        {
            this.SqlQuery(sqlstr, parameters);
            return this.ExecuteNonQuery();

        }
        #endregion

        #region 事务操作        
        private bool IsTranslation { get; set; }
        protected IDbTransaction Translation { get; private set; }
        public void BeginTranslation()
        {
            this.Translation = this.Connection.BeginTransaction();
        }
        public void BeginTranslation(IsolationLevel level)
        {
            this.Translation = this.Connection.BeginTransaction(level);
        }
        public void Commit()
        {
            this.Translation.Commit();
            this.Translation.Dispose();
        }
        public void Rollback()
        {
            this.Translation.Rollback();
            this.Translation.Dispose();
        }
        #endregion
        public abstract DataTable ExcuteDbProcedure();

        public abstract IDataReader ExcuteDataReaders();

        public abstract DataTable ExcuteDataTable();

        public abstract int ExecuteNonQuery();

        public abstract object ExecuteScalar();

        public virtual void Dispose()
        {
            if (this.Translation != null)
            {
                this.Rollback();
            }
            if (this.Connection.State == ConnectionState.Open)
            {
                this.Connection.Close();
            }
        }

    }
    public abstract class BaseOrm : IOrm
    {
        private IDbConnection _connection;
        protected BaseOrm(IDbConnection connection)
        {
            this.Connection = connection;
        }
        protected IDbConnection Connection
        {
            get
            {
                if (this._connection.State != ConnectionState.Open)
                {
                    this._connection.Open();
                }
                return this._connection;
            }
            private set
            {
                this._connection = value;
            }
        }
        /// <summary>
        ///  存储Sql 和 参数
        /// </summary>
        protected Tuple<string, DbParameter[]> _query = Tuple.Create<string, DbParameter[]>(null, null);
        public virtual BaseOrm SqlQuery(string sqlstr)
        {
            return this.SqlQuery(sqlstr, null);
        }
        public virtual BaseOrm SqlQuery(string sqlstr, params DbParameter[] parameters)
        {
            if (this.IsQueryEmpty())
            {
                throw new Exception("SqlQuery执行多次");
            }
            this._query = new Tuple<string, DbParameter[]>(sqlstr, parameters);
            return this;
        }
        public virtual BaseOrm SqlQuery(string sqlstr, params object[] parameters)
        {
            if (!this.IsQueryEmpty())
            {
                throw new Exception("SqlQuery执行多次");
            }
            this._query = new Tuple<string, DbParameter[]>(sqlstr, GetDbParameters(parameters));
            return this;
        }
        /// <summary>
        /// 将Object 数组转换成Parameter 例：DbParameter(@0,value) 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private DbParameter[] GetDbParameters(params object[] parameters)
        {
            var count = parameters.Count();
            var dbParameters = new DbParameter[count];
            for (int i = 0; i < count; i++)
            {
                dbParameters[i] = this.BuildDbParameter(i.ToString(), parameters[i]);
            }
            return dbParameters;
        }
        protected virtual DbParameter BuildDbParameter(string parameterName, object value)
        {
            throw new NotImplementedException();
        }
        protected virtual DbParameter GetDbParameter(string parameterName, object value)
        {
            throw new NotImplementedException();
        }
        public virtual IDbCommand GetDbCommand(string sqlStr, DbParameter[] parameters)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 清空执行列表
        /// </summary>
        protected void ClearQuery()
        {
            this._query = Tuple.Create<string, DbParameter[]>(null, null);
        }
        protected bool IsQueryEmpty()
        {
            if (_query == Tuple.Create<string, DbParameter[]>(null, null))
            {
                return true;
            }
            return false;
        }

        #region 事务操作        
        private bool IsTranslation { get; set; }
        protected IDbTransaction Translation { get; private set; }
        public void BeginTranslation()
        {
            this.Translation = this.Connection.BeginTransaction();
        }
        public void BeginTranslation(IsolationLevel level)
        {
            this.Translation = this.Connection.BeginTransaction(level);
        }
        public void Commit()
        {
            this.Translation.Commit();
            this.Translation.Dispose();
        }
        public void Rollback()
        {
            this.Translation.Rollback();
            this.Translation.Dispose();
        }
        #endregion

        public List<T> ToEntityList<T>()
        {
            var data = this.ExcuteDataTable();
            List<T> entitylist = new List<T>();
            for (int i = 0; i < data.Rows.Count; i++)
            {
                entitylist.Add(DataRowConvert.ToEntity<T>(data.Rows[i]));
            }
            return entitylist;
        }
        public T ToEntity<T>()
        {
            var dt = this.ExcuteDataTable();
            if (dt.Rows.Count == 0)
            {
                return default(T);
            }
            if (dt.Columns.Count == 1)
            {
                return (T)Convert.ChangeType(dt.Rows[0][0], typeof(T));
            }
            return DataRowConvert.ToEntity<T>(dt.Rows[0]);
        }
   
        /// <summary>
        /// 执行Entity操作
        /// </summary>
        /// <param name="sqlstr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private int CommnEntityExecute(string sqlstr, DbParameter[] parameters)
        {
            this.SqlQuery(sqlstr, parameters);
            return this.ExecuteNonQuery();

        }

        public abstract IDataReader ExcuteDataReaders();

        public abstract DataTable ExcuteDataTable();

        public abstract int ExecuteNonQuery();

        public abstract object ExecuteScalar();
        public abstract DataTable ExcuteDbProcedure();

        public virtual void Dispose()
        {
            if (this.Translation != null)
            {
                this.Rollback();
            }
            if (this.Connection.State == ConnectionState.Open)
            {
                this.Connection.Close();
            }
        }
    }
}
