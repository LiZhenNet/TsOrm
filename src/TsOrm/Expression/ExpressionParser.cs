
namespace TsOrm.Expression
{
    /// <summary> 
    /// 表达式树解析抽象泛型类
    /// </summary>
    public abstract class ExpressionParser<T> : IExpressionParser
        where T : System.Linq.Expressions.Expression
    {
        public abstract void Select(T expr, ParserArgs args);
        public abstract void Where(T expr, ParserArgs args);
        public void Select(System.Linq.Expressions.Expression expr, ParserArgs args)
        {
            Select((T)expr, args);
        }
        public void Where(System.Linq.Expressions.Expression expr, ParserArgs args)
        {
            Where((T)expr, args);
        }
    }
}
