using System;
using System.Linq.Expressions;

namespace TsOrm.Expression.Paser
{
    class BinaryExpressionParser : ExpressionParser<BinaryExpression>
    {
        public override void Where(BinaryExpression expr, ParserArgs args)
        {
            if (ExistsBracket(expr.Left))
            {
                args.Builder.Append(' ');
                args.Builder.Append('(');
                Parser.Where(expr.Left, args);
                args.Builder.Append(')');
            }
            else
            {
                Parser.Where(expr.Left, args);
            }
            Sign(expr.NodeType, args);
            if (ExistsBracket(expr.Right))
            {
                args.Builder.Append(' ');
                args.Builder.Append('(');
                Parser.Where(expr.Right, args);
                args.Builder.Append(')');

            }
            else
            {
                Parser.Where(expr.Right, args);
            }
        }
        /// <summary> 
        /// 判断是否需要添加括号
        /// </summary>
        private static bool ExistsBracket(System.Linq.Expressions.Expression expr)
        {
            var s = expr.ToString();
            return s != null && s.Length > 5 && s[0] == '(' && s[1] == '(';
        }

        private static void Sign(ExpressionType type, ParserArgs args)
        {
            switch (type)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    args.Builder.Append(" AND");
                    break;
                case ExpressionType.Equal:
                    args.Builder.Append(" =");
                    break;
                case ExpressionType.GreaterThan:
                    args.Builder.Append(" >");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    args.Builder.Append(" >=");
                    break;
                case ExpressionType.NotEqual:
                    args.Builder.Append(" <>");
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    args.Builder.Append(" OR");
                    break;
                case ExpressionType.LessThan:
                    args.Builder.Append(" <");
                    break;
                case ExpressionType.LessThanOrEqual:
                    args.Builder.Append(" <=");
                    break;
                default:
                    throw new NotImplementedException("无法解释节点类型" + type);
            }
        }

        public override void Select(BinaryExpression expr, ParserArgs args)
        {
            throw new NotImplementedException();
        }
            
    }
}
