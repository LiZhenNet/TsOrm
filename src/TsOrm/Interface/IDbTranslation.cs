using System.Data;

namespace TsOrm.Interface
{
   public interface IDbTranslation
    {
        void BeginTranslation(IsolationLevel level);
        void BeginTranslation();
        void Commit();
        void Rollback();
    }
}
                            