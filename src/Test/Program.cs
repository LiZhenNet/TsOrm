using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using TsOrm;
using TsOrm.Attrbute;
using TsOrm.Interface;
using static TsOrm.OrmFactory;

namespace Test_v0._2
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionstring = "Data Source=.\\SQLEXPRESS;uid=sa;pwd=root;Initial Catalog=test;Integrated Security=SSPI;Integrated Security=True";
            IOrm orm = OrmFactory.GetOrm(ServerType.SqlServer, connectionstring);
            IOrm<User> torm = OrmFactory.GetOrm<User>(ServerType.SqlServer, connectionstring);
            string sql = "SELECT top 1000 dc.collection_id,dc.collection_name FROM data_collection dc ";
            //string mysql = "SELECT dc.collection_id,dc.collection_name FROM data_collection dc LIMIT 0,1000";
            /*===========================================================================================*/
            Stopwatch s = new Stopwatch();
            s.Start();
            //DataSet ds= MySqlHelper.ExecuteDataset(con, sql);
            //s.Stop();
            //Console.WriteLine(ds.Tables[0].Rows.Count+"条,MySqlHelper DataSet:" + s.ElapsedMilliseconds);
            /*===========================================================================================*/
            s.Restart();
            DataTable t = orm.SqlQuery(sql).ExcuteDataTable();
            s.Stop();
            Console.WriteLine(t.Rows.Count + "条,返回DataTable:" + s.ElapsedMilliseconds);
            /*===========================================================================================*/
            s.Restart();
            List<collection> list = orm.SqlQuery(sql).ToEntityList<collection>();
            s.Stop();
            Console.WriteLine(list.Count + "条,返回List:" + s.ElapsedMilliseconds);
            s.Restart();
            /*===========================================================================================*/
            List<collection> list2 = orm.SqlQuery(sql).ToEntityList<collection>();
            s.Stop();
            Console.WriteLine(list2.Count + "条,第二次List:" + s.ElapsedMilliseconds);
            /*===========================================================================================*/

            //*更新*/
            int count = torm.UpdateColumn("NickName", "TsOrm").Where("Uid=89480").Update();
            //*更新*/
            int count2 = torm.UpdateColumn("NickName", "TsOrm2").Where(x => x.Uid == 89480).Update();
            //*更新*/
            int count3 = torm.UpdateColumn(x => x.NickName, "TsOrm").Where(x => x.Uid == 89480).Update();
            int ccc = count;
            int ccc2 = count2;
            int ccc3 = count3;
            Console.ReadKey();
        }
        [Table("data_collection")]
        public class collection
        {
            public string collection_id { get; set; }
            public string collection_name { get; set; }
        }

        [Table("data_user")]
        public class User
        {
            public long Uid { get; set; }
            public string NickName { get; set; }
        }
    }
}
