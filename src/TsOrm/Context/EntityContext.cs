using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using TsOrm.Attrbute;

namespace TsOrm.Context
{
    /// <summary>
    /// 实体上下文,用于获取表名,列信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal partial class EntityContext<T>
    {
        static EntityContext()
        {
            Type = typeof(T);
            TableName = GetTableName();
            Columns = GetColumns();
        }
        /// <summary>
        /// TableName
        /// </summary>
        public static string TableName { get; private set; }
        /// <summary>
        /// 列信息
        /// </summary>
        public static IList<ColumnInfo<T>> Columns { get; private set; }
        /// <summary>
        /// Class的Type
        /// </summary>
        public static Type Type { get; private set; }
        /// <summary>
        /// 获取表名
        /// </summary>
        /// <returns></returns>
        private static string GetTableName()
        {
            var attributes = Type.GetCustomAttributes(false);
            var attribute = attributes.OfType<TableAttribute>().FirstOrDefault();
            if (attribute == null)
            {
                return Type.Name;
            }
            return attribute.TableName;
        }
        /// <summary>
        /// 获取列
        /// </summary>
        /// <returns></returns>
        private static IList<ColumnInfo<T>> GetColumns()
        {
            var columns = new List<ColumnInfo<T>>();
            var properties = Type.GetProperties();
            foreach (var p in properties)
            {
                if (p.GetSetMethod() == null || p.GetGetMethod() == null)
                {
                    continue;
                }
                var ci = new ColumnInfo<T>(){ Property = p };
                var attribute = p.GetCustomAttributes().OfType<ColumnAttribute>().FirstOrDefault();
                if ( attribute == null)
                {
                    ci.ColumnName = p.Name;
                    ci.Key = false;
                    ci.Ignore = false;
                }
                else
                {
                    ci.ColumnName = attribute.ColumnName;
                    ci.Key = false;
                    ci.Ignore = attribute.Ignore;
                }
                ci.GetterDelegate = CreateGetterDelegate(p);
                columns.Add(ci);
            }
            return columns;
        }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private static Func<T, string> CreateGetterDelegate(PropertyInfo info)
        {
            var dynamicMethod = new DynamicMethod(Guid.NewGuid().ToString("N") + "_GetterDelegate", typeof(string), new[] { typeof(T) });
            var il = dynamicMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, info.GetGetMethod());
            il.Emit(OpCodes.Call, typeof(Convert).GetMethod("ToString", new Type[] { info.PropertyType }));
            il.Emit(OpCodes.Ret);
            return (Func<T, string>)dynamicMethod.CreateDelegate(typeof(Func<T, string>));
        }
    }
    /// <summary>
    /// 行信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ColumnInfo<T>
    {
        /// <summary>
        /// 属性信息
        /// </summary>
        public PropertyInfo Property { get; set; }
        /// <summary>
        /// 对应列名
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 忽略
        /// </summary>
        public bool Ignore { get; set; }
        /// <summary>
        /// 是否为主键
        /// </summary>
        public bool Key { get; set; }
        /// <summary>
        /// 获取行值
        /// </summary>
        public Func<T, string> GetterDelegate { get; set; }
    }
}
