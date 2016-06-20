
using System;

namespace TsOrm.Extend
{
    /// <summary>
    /// object扩展方法
    /// </summary>
    public static class ObjectExtend
    {
        /// <summary>
        /// 判断是否为Null
        /// </summary>
        /// <param name="obj">Object类型</param>
        /// <returns></returns>
        public static bool IsNull(this object obj)
        {
            if (obj == null)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 返回Int类型(注意：object为空,转换出现异常返回 0) 
        /// </summary>
        /// <param name="obj">Object类型</param>
        /// <returns></returns>
        public static int ToInt(this object obj)
        {
            if (obj == null)
            {
                return 0;
            }
            try
            {
                int n = Convert.ToInt32(obj);
                return n;
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 返回Long类型(注意：object为空,转换出现异常返回 0) 
        /// </summary>
        /// <param name="obj">Object类型</param>
        /// <returns></returns>
        public static long ToLong(this object obj)
        {
            if (obj == null)
            {
                return 0;
            }
            try
            {
                long n = Convert.ToInt64(obj);
                return n;
            }
            catch
            {
                return 0;
            }
        }
    }
}
