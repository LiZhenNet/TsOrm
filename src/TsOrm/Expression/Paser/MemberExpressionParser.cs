using System;
using System.Linq.Expressions;

namespace TsOrm.Expression.Paser
{
    class MemberExpressionParser : ExpressionParser<MemberExpression>
    {
        public override void Where(MemberExpression expr, ParserArgs args)
        {
            Parser.Where(expr.Expression, args);
            args.Builder.Append(expr.Member.Name);
        }
        public override void Select(MemberExpression expr, ParserArgs args)
        {
            args.Builder.Append(expr.Member.Name);
        }
    }

}
