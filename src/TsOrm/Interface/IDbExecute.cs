using System.Data;

namespace TsOrm.Interface
{
   public interface IDbExecute
    {
        int ExecuteNonQuery();
        object ExecuteScalar();
        DataTable ExcuteDataTable();
        DataTable ExcuteDbProcedure();
        IDataReader ExcuteDataReaders();
    }
}
