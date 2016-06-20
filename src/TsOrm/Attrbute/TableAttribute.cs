using System;

namespace TsOrm.Attrbute
{
    /// <summary>
    /// Table标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TableAttribute : Attribute
    {
        /// <summary>
        /// 与数据库对应表名
        /// </summary>
        public string TableName { get; set; }
        public TableAttribute(string tableName)
        {
            this.TableName = tableName;
        }
    }
}
