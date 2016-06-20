using System;
using System.Linq.Expressions;

namespace TsOrm.Expression.Paser
{
    class ParameterExpressionParser : ExpressionParser<ParameterExpression>
    {
        public override void Where(ParameterExpression expr, ParserArgs args)
        {
            args.Builder.Append(' ');
        }
        public override void Select(ParameterExpression expr, ParserArgs args)
        {
            throw new NotImplementedException();
        }
    }

}
