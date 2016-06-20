using System;
using System.Linq.Expressions;


namespace TsOrm.Expression
{
    /// <summary> 
    /// 解析器静态对象 
    /// </summary>
    public static class Parser
    {
        private static readonly IExpressionParser[] Parsers = InitParsers();
        /// <summary>
        /// 初始化所有表达式Paser
        /// </summary>
        /// <returns></returns>
        static IExpressionParser[] InitParsers()
        {
            var codes = Enum.GetValues(typeof(ExpressionTypeCode));
            var parsers = new IExpressionParser[codes.Length];
            foreach (ExpressionTypeCode code in codes)
            {
                if (code.ToString().EndsWith("Expression"))
                {
                    var type = Type.GetType("TsOrm.Expression.Paser." + code + "Parser");
                    if (type != null)
                    {
                        parsers[(int)code] = (IExpressionParser)Activator.CreateInstance(type);
                    }
                }
            }
            return parsers;
        }
        /// <summary> 
        /// 得到表达式类型的枚举对象 
        /// </summary>
        /// <param name="expr"> 扩展对象:Expression </param>
        /// <returns> </returns>
        public static ExpressionTypeCode GetCodeType(System.Linq.Expressions.Expression expr)
        {
            if (expr == null)
            {
                return ExpressionTypeCode.Null;
            }
            if (expr is BinaryExpression)
            {
                return ExpressionTypeCode.BinaryExpression;
            }
            if (expr is BlockExpression)
            {
                return ExpressionTypeCode.BlockExpression;
            }
            if (expr is ConditionalExpression)
            {
                return ExpressionTypeCode.ConditionalExpression;
            }
            if (expr is ConstantExpression)
            {
                return ExpressionTypeCode.ConstantExpression;
            }
            if (expr is DebugInfoExpression)
            {
                return ExpressionTypeCode.DebugInfoExpression;
            }
            if (expr is DefaultExpression)
            {
                return ExpressionTypeCode.DefaultExpression;
            }
            if (expr is DynamicExpression)
            {
                return ExpressionTypeCode.DynamicExpression;
            }
            if (expr is GotoExpression)
            {
                return ExpressionTypeCode.GotoExpression;
            }
            if (expr is IndexExpression)
            {
                return ExpressionTypeCode.IndexExpression;
            }
            if (expr is InvocationExpression)
            {
                return ExpressionTypeCode.InvocationExpression;
            }
            if (expr is LabelExpression)
            {
                return ExpressionTypeCode.LabelExpression;
            }
            if (expr is LambdaExpression)
            {
                return ExpressionTypeCode.LambdaExpression;
            }
            if (expr is ListInitExpression)
            {
                return ExpressionTypeCode.ListInitExpression;
            }
            if (expr is LoopExpression)
            {
                return ExpressionTypeCode.LoopExpression;
            }
            if (expr is MemberExpression)
            {
                return ExpressionTypeCode.MemberExpression;
            }
            if (expr is MemberInitExpression)
            {
                return ExpressionTypeCode.MemberInitExpression;
            }
            if (expr is MethodCallExpression)
            {
                return ExpressionTypeCode.MethodCallExpression;
            }
            if (expr is NewArrayExpression)
            {
                return ExpressionTypeCode.NewArrayExpression;
            }
            if (expr is NewExpression)
            {
                return ExpressionTypeCode.NewArrayExpression;
            }
            if (expr is ParameterExpression)
            {
                return ExpressionTypeCode.ParameterExpression;
            }
            if (expr is RuntimeVariablesExpression)
            {
                return ExpressionTypeCode.RuntimeVariablesExpression;
            }
            if (expr is SwitchExpression)
            {
                return ExpressionTypeCode.SwitchExpression;
            }
            if (expr is TryExpression)
            {
                return ExpressionTypeCode.TryExpression;
            }
            if (expr is TypeBinaryExpression)
            {
                return ExpressionTypeCode.TypeBinaryExpression;
            }
            if (expr is UnaryExpression)
            {
                return ExpressionTypeCode.UnaryExpression;
            }
            return ExpressionTypeCode.Unknown;
        }
        /// <summary> 
        /// 得到当前表达式对象的解析组件
        /// </summary>
        /// <param name="expr"> 扩展对象:Expression </param>
        /// <returns> </returns>
        public static IExpressionParser GetParser(System.Linq.Expressions.Expression expr)
        {
            var codetype = GetCodeType(expr);
            var parser = Parsers[(int)codetype];
            if (parser == null)
            {
                switch (codetype)
                {
                    case ExpressionTypeCode.Unknown:
                        throw new ArgumentException("未知的表达式类型", "expr");
                    case ExpressionTypeCode.Null:
                        throw new ArgumentNullException("expr", "表达式为空");
                    default:
                        throw new NotImplementedException("尚未实现" + codetype + "的解析");
                }
            }
            return parser;
        }
        /// <summary>
        /// Select 
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="args"></param>
        public static void Select(System.Linq.Expressions.Expression expr, ParserArgs args)
        {
            GetParser(expr).Select(expr, args);
        }
        /// <summary>
        /// Where
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="args"></param>
        public static void Where(System.Linq.Expressions.Expression expr, ParserArgs args)
        {
            GetParser(expr).Where(expr, args);
        }

    }
    /// <summary> 
    /// 表达式类型枚举
    /// </summary>
    public enum ExpressionTypeCode
    {
        /// <summary> 
        /// 未知类型表达式
        /// </summary>
        Unknown = 0,
        /// <summary> 
        /// 空表达式 null
        /// </summary>
        Null = 1,
        /// <summary> 
        /// 表示包含二元运算符的表达式。
        /// </summary>
        BinaryExpression = 2,
        /// <summary> 
        /// 表示一个包含可在其中定义变量的表达式序列的块。
        /// </summary>
        BlockExpression = 3,
        /// <summary>
        ///  表示包含条件运算符的表达式。
        /// </summary>
        ConditionalExpression = 4,
        /// <summary>
        ///  表示具有常量值的表达式。
        /// </summary>
        ConstantExpression = 5,
        /// <summary> 
        /// 发出或清除调试信息的序列点。 这允许调试器在调试时突出显示正确的源代码。
        /// </summary>
        DebugInfoExpression = 6,
        /// <summary> 
        /// 表示类型或空表达式的默认值。
        /// </summary>
        DefaultExpression = 7,
        /// <summary> 表示动态操作。
        /// </summary>
        DynamicExpression = 8,
        /// <summary> 
        /// 表示无条件跳转。 这包括 return 语句、break 和 continue 语句以及其他跳转。
        /// </summary>
        GotoExpression = 9,
        /// <summary> 
        /// 表示编制属性或数组的索引。
        /// </summary>
        IndexExpression = 10,
        /// <summary>
        ///  表示将委托或 lambda 表达式应用于参数表达式列表的表达式。
        /// </summary>
        InvocationExpression = 11,
        /// <summary> 
        /// 表示一个标签，可以将该标签放置在任何 Expression 上下文中。 
        /// </summary>
        LabelExpression = 12,
        /// <summary> 
        /// 描述一个 lambda 表达式。 这将捕获与 .NET 方法体类似的代码块。
        /// </summary>
        LambdaExpression = 13,
        /// <summary> 
        /// 表示包含集合初始值设定项的构造函数调用。
        /// </summary>
        ListInitExpression = 14,
        /// <summary> 
        /// 表示无限循环。 可以使用“break”退出它。
        /// </summary>
        LoopExpression = 15,
        /// <summary> 
        /// 表示访问字段或属性。
        /// </summary>
        MemberExpression = 16,
        /// <summary> 
        /// 表示调用构造函数并初始化新对象的一个或多个成员。
        /// </summary>
        MemberInitExpression = 17,
        /// <summary> 
        /// 表示对静态方法或实例方法的调用。
        /// </summary>
        MethodCallExpression = 18,
        /// <summary> 
        /// 表示创建新数组并可能初始化该新数组的元素。
        /// </summary>
        NewArrayExpression = 19,
        /// <summary> 
        /// 表示构造函数调用。
        /// </summary>
        NewExpression = 20,
        /// <summary> 
        /// 表示命名的参数表达式。
        /// </summary>
        ParameterExpression = 21,
        /// <summary> 
        /// 一个为变量提供运行时读/写权限的表达式。
        /// </summary>
        RuntimeVariablesExpression = 22,
        /// <summary> 
        /// 表示一个控制表达式，该表达式通过将控制传递到 SwitchCase 来处理多重选择。
        /// </summary>
        SwitchExpression = 23,
        /// <summary> 
        /// 表示 try/catch/finally/fault 块。
        /// </summary>
        TryExpression = 24,
        /// <summary>
        ///  表示表达式和类型之间的操作。
        /// </summary>
        TypeBinaryExpression = 25,
        /// <summary> 
        /// 表示包含一元运算符的表达式。
        /// </summary>
        UnaryExpression = 26,
    }
}
