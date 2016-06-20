using System;

namespace TsOrm.Attrbute
{
    /// <summary>
    /// 列标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        /// 与数据库对应列名
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 是否忽略
        /// </summary>
        public bool Ignore { get; set; }
        public ColumnAttribute(string columnName)
            : this(columnName, false)                
        {
        }
        public ColumnAttribute(string columnName, bool Ignore)
        {
            this.ColumnName = columnName;
            this.Ignore = Ignore;
        }
    }
    /// <summary>
    /// 需要Json反序列化的列标记
    /// </summary>
    public class JsonNetAttribute : Attribute
    {

    }

}
