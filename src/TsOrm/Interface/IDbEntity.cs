using System.Collections.Generic;
using System.Data;

namespace TsOrm.Interface
{
   public interface IDbEntity<T>
    {
        List<T> ToEntityList();
        List<T> ToEntityList(CommandType cmdtype);
        T ToEntity();
        T QueryEntityByKey(string key);
        int InsertEntity(T model);
        int UpdateEntity(T model);
        int Delete(T model);
        int Delete(string keyValue);

    }
}
