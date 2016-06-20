using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using TsOrm.Attrbute;
using TsOrm.Context;
using Newtonsoft.Json;
using System.Linq;
using System.ComponentModel;

namespace TsOrm.Core
{
    /// <summary>
    /// 把DataRow转换成实体T
    /// </summary>
    public class DataRowConvert
    {
        /// <summary>
        /// 添加Cache锁
        /// </summary>
        private readonly static object lockobj = new object();
        //把DataRow转换为对象的委托声明
        private delegate T Convert<T>(DataRow dataRecord);
        /// <summary>
        /// DataRow转换成Entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T ToEntity<T>(DataRow dr)
        {
            var type = typeof(T);
            if (type == typeof(Int32) || type == typeof(Int64) || type == typeof(string))
            {
                return (T)Convert.ChangeType(dr[0], type);
            }
            StringBuilder key = new StringBuilder();
            key.Append(type.Name + "_");
            for (int index = 0; index < dr.Table.Columns.Count; index++)
            {
                key.Append(dr.Table.Columns[index].ColumnName);
            }
            var columns = EntityContext<T>.Columns;
            if (dr == null)
                return default(T);
            Convert<T> Converter = null;
            if (!Cache.ContainsKey(key.ToString()))
            {
                DynamicMethod method = new DynamicMethod(key.ToString(), type, new Type[] { typeof(DataRow) }, type, true);
                ILGenerator generator = method.GetILGenerator();
                LocalBuilder result = generator.DeclareLocal(type);
                generator.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
                generator.Emit(OpCodes.Stloc, result);
                for (int index = 0; index < columns.Count; index++)
                {
                    PropertyInfo propertyInfo = type.GetProperty(columns[index].ColumnName);
                    Label endIfLabel = generator.DefineLabel();
                    if (dr.Table.Columns.Contains(propertyInfo.Name) && !columns[index].Ignore)
                    {
                        int rindex = dr.Table.Columns.IndexOf(propertyInfo.Name);
                        if (rindex < 0)
                        {
                            continue;
                        }
                        //把参数推到堆栈中
                        generator.Emit(OpCodes.Ldarg_0); 
                        generator.Emit(OpCodes.Ldc_I4, rindex);
                        //dr.IsNull(rindex)
                        generator.Emit(OpCodes.Callvirt, isDBNullMethod);
                        generator.Emit(OpCodes.Brtrue, endIfLabel);
                        //将指定索引处的局部变量加载到计算堆栈上。
                        generator.Emit(OpCodes.Ldloc, result);
                        generator.Emit(OpCodes.Ldarg_0);
                        generator.Emit(OpCodes.Ldc_I4, rindex);
                        
                        generator.Emit(OpCodes.Callvirt, getValueMethod);
                        if (propertyInfo.PropertyType.IsValueType)
                        {
                            MethodInfo x = ConvertMethods[propertyInfo.PropertyType];
                            generator.Emit(OpCodes.Call, x);
                        }
                        else
                        {
                            if (propertyInfo.PropertyType == typeof(string))
                            {
                                generator.Emit(OpCodes.Call, ConvertMethods[propertyInfo.PropertyType]);
                            }
                            else
                            {
                                if (propertyInfo.GetCustomAttribute(typeof(JsonNetAttribute)) != null)
                                {
                                    var _normalmethod = typeof(JsonConvert).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "DeserializeObject" && m.IsGenericMethod).FirstOrDefault();
                                    MethodInfo _genericmethod = _normalmethod.MakeGenericMethod(new Type[] { propertyInfo.PropertyType});
                                    generator.Emit(OpCodes.Call, _genericmethod);
                                }
                                else
                                {
                                    generator.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
                                }
                            }
                        }
                        generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
                        generator.MarkLabel(endIfLabel);
                    }
                }
                generator.Emit(OpCodes.Ldloc, result);
                generator.Emit(OpCodes.Ret);
                Converter = (Convert<T>)method.CreateDelegate(typeof(Convert<T>));
                //加锁 并发高时 出现插入键重复
                lock (lockobj)
                {
                    if (!Cache.ContainsKey(key.ToString()))
                    {
                        Cache.Add(key.ToString(), Converter);
                    }
                }
            }
            else
            {
                Converter = (Convert<T>)Cache[key.ToString()];
            }
            return Converter(dr);
        }
        private static readonly MethodInfo getValueMethod = typeof(DataRow).GetMethod("get_Item", new Type[] { typeof(int) });
        private static readonly MethodInfo isDBNullMethod = typeof(DataRow).GetMethod("IsNull", new Type[] { typeof(int) });
        private static Dictionary<string, Delegate> Cache = new Dictionary<string, Delegate>();
        private static Dictionary<Type, MethodInfo> ConvertMethods = new Dictionary<Type, MethodInfo>()
        {
           {typeof(int),typeof(Convert).GetMethod("ToInt32",new Type[]{typeof(object)})},
           {typeof(int?),typeof(DataRowConvert).GetMethod("NullableInt",new Type[]{typeof(object)})},
           {typeof(Int16),typeof(Convert).GetMethod("ToInt16",new Type[]{typeof(object)})},
           {typeof(Int64),typeof(Convert).GetMethod("ToInt64",new Type[]{typeof(object)})},
           {typeof(Nullable<Int64>),typeof(DataRowConvert).GetMethod("NullableInt64",new Type[]{typeof(object)})},
           {typeof(DateTime),typeof(Convert).GetMethod("ToDateTime",new Type[]{typeof(object)})},
           {typeof(decimal),typeof(Convert).GetMethod("ToDecimal",new Type[]{typeof(object)})},
           {typeof(Double),typeof(Convert).GetMethod("ToDouble",new Type[]{typeof(object)})},
           {typeof(Boolean),typeof(Convert).GetMethod("ToBoolean",new Type[]{typeof(object)})},
           {typeof(string),typeof(Convert).GetMethod("ToString",new Type[]{typeof(object)})}
       };
        public static Nullable<int> NullableInt( object obj)
        {
            NullableConverter converter = new NullableConverter(typeof(int?));
            return (int?)converter.ConvertFromString(obj.ToString());
        }
        public static Nullable<Int64> NullableInt64(object obj)
        {
            NullableConverter converter = new NullableConverter(typeof(Int64?));
            return (Int64?)converter.ConvertFromString(obj.ToString());
        }
    }
}
