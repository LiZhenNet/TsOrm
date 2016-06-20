using System;
using System.Linq.Expressions;

namespace TsOrm.Expression.Paser
{
    class ConstantExpressionParser : ExpressionParser<ConstantExpression>
    {
        public override void Where(ConstantExpression expr, ParserArgs args)
        {
            args.Builder.Append(' ');
            var val = expr.Value;
            if (val == null || val == DBNull.Value)
            {
                args.Builder.Append("NULL");
                return;
            }
            if (val is bool)
            {
                args.Builder.Append(val.GetHashCode());
                return;
            }
            var code = (int)Type.GetTypeCode(val.GetType());
            if (code >= 5 && code <= 15)            //如果expr.Value是数字类型
            {
                args.Builder.Append(val);
            }
            else
            {
                args.Builder.Append('\'');
                args.Builder.Append(val);
                args.Builder.Append('\'');
            }
        }

        public override void Select(ConstantExpression expr, ParserArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
