using System;
using TsOrm.Interface;
using TsOrm.Orm;

namespace TsOrm
{
    public static class OrmFactory
    {
        /// <summary>
        /// 获取Orm
        /// </summary>
        /// <param name="dbtype">数据库类型</param>
        /// <param name="connectionstring">连接字符串</param>
        /// <returns></returns>
        public static IOrm GetOrm(ServerType dbtype,string connectionstring)
        {
            switch (dbtype)
            {
                case ServerType.SqlServer:
                    return new SqlServerOrm(connectionstring);
                case ServerType.MySql:
                    return new MySqlOrm(connectionstring);
                case ServerType.PostgreSql:
                    return new PostgreSqlOrm(connectionstring);
                default:
                    throw new ArgumentException("数据库不支持");
            }
        }
        /// <summary>
        /// 获取Orm
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbtype">数据库类型</param>
        /// <param name="connectionstring">连接字符串</param>
        /// <returns></returns>
        public static IOrm<T> GetOrm<T>(ServerType dbtype, string connectionstring)
        {

            switch (dbtype)
            {
                case ServerType.SqlServer:
                    return new SqlServerOrm<T>(connectionstring);
                case ServerType.MySql:
                    return new MySqlOrm<T>(connectionstring);
                case ServerType.PostgreSql:
                    return new PostgreSqlOrm<T>(connectionstring);
                default:
                    throw new ArgumentException("数据库不支持");
            }
        }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public enum ServerType
        {
            SqlServer=0,
            MySql=1,
            PostgreSql=2
        }
    }
}
