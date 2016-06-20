
using System;
using System.Data.Common;
using System.Linq.Expressions;
using TsOrm.Core;

namespace TsOrm.Interface
{
    public interface IOrm:IDbExecute,IDbTranslation,IDisposable
    {
        BaseOrm SqlQuery(string sqlstr);
        BaseOrm SqlQuery(string sqlstr, params DbParameter[] parameters);
        BaseOrm SqlQuery(string sqlstr, params object[] parameters);
    }

    public interface IOrm<T>: IDbExecute, IDbEntity<T>, IDbTranslation, IDisposable
    {
        BaseOrm<T> UpdateColumn(string columnname, object value);
        BaseOrm<T> UpdateColumn<TField>(Expression<Func<T, TField>> expr, object value);
        BaseOrm<T> Where(string wherecondition); 
        BaseOrm<T> Where(Expression<Func<T, bool>> expr);
        BaseOrm<T> SqlQuery(string sqlstr);
        BaseOrm<T> SqlQuery(string sqlstr, params DbParameter[] parameters);
        BaseOrm<T> SqlQuery(string sqlstr, params object[] parameters);
    }
}
