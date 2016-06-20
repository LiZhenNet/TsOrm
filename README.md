##**TsOrm**
**TsOrm是为了学习DynamicMethod和Lambda表达式而写出来的简易的ORM,开发时参考了一些博客园大神的文章,如果你不想使用EF,NH等重量级Orm,欢迎大家学习和使用TsOrm！**

**没写单元测试,目前主要在Mysql使用,还未发现什么Bug**
 
###How To Use
TsOrm使用非常简单,代码如下：
####创建IOrm , TsOrm 支持 SqlServer、MySql、PostgreSQL 
```
    string connectionstring ="DataSource=.\\SQLEXPRESS;uid=sa;pwd=root;Initial Catalog=test;Integrated Security=SSPI;Integrated Security=True";
    //SqlServer
    IOrm Sqlserver = OrmFactory.GetOrm(ServerType.SqlServer, connectionstring);
    //MySql
    IOrm MySql = OrmFactory.GetOrm(ServerType.SqlServer, connectionstring);
```
#### 执行SQL
##### 查询操作
```
    string sql = "SELECT * FROM table";
    //返回第一行第一列
    object obj = orm.SqlQuery(sql).ExecuteScalar();
    // 返回 DataTable
    DataTable table = Sqlserver.SqlQuery(sql).ExcuteDataTable();
    // 返回List<T>
    List<Entity> list = Sqlserver.SqlQuery(sql).ToEntityList<Entity>();
```
##### 增删改操作
```
    string sql = "DELETE  FROM table";
    int n =orm.SqlQuery(sql).ExecuteNonQuery();
```
##### Interesting Things 
```
    [Table("data_user")]
    public class User
    {
        public long Uid { get; set; }
        public string NickName { get; set; }
    }
    IOrm<User> orm = OrmFactory.GetOrm<User>(ServerType.SqlServer, connectionstring);
    //查找
    List<User> user = orm.SqlQuery(sql).ToEntityList();
    //执行存储过程
    List<User> result = orm.SqlQuery(sql,parameter).ToEntityList(CommandType.StoredProcedure);
    //可以这么更新
    int count = orm.UpdateColumn("NickName", "TsOrm").Where("Uid=89480").Update();
    //可以这么使用Lambda更新
    int count2 = orm.UpdateColumn("NickName", "TsOrm2").Where(x => x.Uid == 89480).Update();
    //可以这么使用Lambda更新
    int count3 = orm.UpdateColumn(x => x.NickName, "TsOrm").Where(x => x.Uid == 89480).Update();
    
    //如果你数据库中有一列储存的Json字符串 你可以在需要Json反序列化的地方加上JsonNet Attribute
    //TsOrm会自动帮你把数据库中的Json字符串反序列化
    public class Entity
    {
        public string name { get; set; }
        [JsonNet]
        public Child child { get; set; }
    }
     public class Child
    {
        public string name { get; set; }
        public int age { get; set; }
    }
    
```
###扩展其他数据库
参考MySqlOrm 继承BaseOrm<T> 或 BaseOrm
重写下列方法:
> * object ExecuteScalar()
> * int ExecuteNonQuery()
> * DataTable ExcuteDbProcedure()
> * DataTable ExcuteDataTable()
> * IDataReader ExcuteDataReaders()
> * IDbCommand GetDbCommand(string sqlStr, DbParameter[] parameters)
> * DbParameter GetDbParameter(string parameterName, object value)