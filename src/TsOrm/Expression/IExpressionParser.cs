
namespace TsOrm.Expression
{
    /// <summary> 
    /// 表达式树解析接口
    /// </summary>
    public interface IExpressionParser
    {
        void Select(System.Linq.Expressions.Expression expr, ParserArgs args);
        void Where(System.Linq.Expressions.Expression expr, ParserArgs args);
    }
}
