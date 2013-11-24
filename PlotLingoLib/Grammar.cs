using PlotLingoLib.Expressions;
using PlotLingoLib.Expressions.Functions;
using PlotLingoLib.Expressions.Values;
using PlotLingoLib.Statements;
using Sprache;
using System.Linq;

namespace PlotLingoLib
{
    /// <summary>
    /// The grammer for the input text language
    /// </summary>
    internal static class Grammar
    {
        /// <summary>
        /// Parse an array of expressions
        /// </summary>
        public static readonly Parser<ArrayValue> ArrayValueParser =
            from values in Parse
                .Ref(() => ExpressionParser)
                .DelimitedBy(Parse.Char(','))
                .Contained(Parse.Char('['), Parse.Char(']'))
            select new ArrayValue(values.ToArray());

        /// <summary>
        /// Our basic identifier, standard.
        /// </summary>
        private static readonly Parser<string> IdentifierParser = (Parse.LetterOrDigit.Or(Parse.Char('_')).Or(Parse.Char('-'))).AtLeastOnce().Text().Token().Named("Identifier");

        /// <summary>
        /// A variable name can be any identifier.
        /// </summary>
        private static readonly Parser<string> VariableNameParser = IdentifierParser;

        /// <summary>
        /// Parse a string
        /// </summary>
        private static readonly Parser<IExpression> StringValueParser =
            (
                from ws in Parse.WhiteSpace.Many()
                from openp in Parse.Char('"')
                from content in Parse.CharExcept('"').Many().Text()
                from closep in Parse.Char('"')
                select new StringValue(content)
            );

        /// <summary>
        /// Parse an identifier that is a variable name.
        /// </summary>
        private static readonly Parser<VariableValue> VariableValueParser =
            (
                from name in VariableNameParser
                select new VariableValue(name)
            );

        /// <summary>
        /// Parse a value (like a number or a string).
        /// </summary>
        private static readonly Parser<IExpression> ValueExpressionParser = StringValueParser;

        /// <summary>
        /// Parser that returns a function.
        /// </summary>
        private static readonly Parser<FunctionExpression> FunctionExpressionParser =
            (
            from fname in IdentifierParser
            from args in ArgumentListParser
            select new FunctionExpression(fname, args)
            );

        /// <summary>
        /// Parse an argument list that goes to a function or similar.
        /// </summary>
        private static readonly Parser<IExpression[]> ArgumentListParser =
            (
            from openp in Parse.Char('(')
            from arg1 in Parse.Ref(() => ExpressionParser)
            from rest in Parse.Char(',').Then(_ => ExpressionParser).Many()
            from closep in Parse.Char(')')
            select new IExpression[] { arg1 }.Concat(rest).ToArray()
            ).Named("Argument List");

        /// <summary>
        /// Parse a method call.
        /// </summary>
        private static readonly Parser<IExpression> MethodExpressionParser =
            (
            from obj in Parse.Ref(() => ExpressionParser)
            from dot in Parse.Char('.')
            from func in FunctionExpressionParser
            select new MethodCallExpression(obj, func)
            ).Named("Method Call");

        /// <summary>
        /// Parse a simple term expression.
        /// </summary>
        private static readonly Parser<IExpression> TermParser =
            (
                from e in VariableValueParser
                .Or(ValueExpressionParser)
                .Or(FunctionExpressionParser)
                select e
            );

        /// <summary>
        /// Parse an expression. Could be a function, or... etc.
        /// </summary>
        private static readonly Parser<IExpression> ExpressionParser =
            (
            from e in FunctionExpressionParser
                .Or(ValueExpressionParser)
                //.Or(MethodExpressionParser)
                .Or(ArrayValueParser)
                .Or(VariableValueParser)
            select e
            );

        /// <summary>
        /// Parse an assignment statement.
        /// </summary>
        private static readonly Parser<IStatement> AssignmentStatementParser =
            (
                from nv in VariableNameParser
                from eq in Parse.Char('=')
                from expr in Parse.Ref(() => ExpressionParser)
                select new AssignmentStatement(nv, expr)
            ).Named("Assignment Statement");

        /// <summary>
        /// Parse an expression statement
        /// </summary>
        private static readonly Parser<IStatement> ExpressionStatementParser =
            (
                from expr in Parse.Ref(() => ExpressionParser)
                select new ExpressionStatement(expr)
            ).Named("Expression Statement");

        /// <summary>
        /// Parse a statement
        /// </summary>
        private static readonly Parser<IStatement> StatementParser =
            (
                from r in AssignmentStatementParser
                    .Or(ExpressionStatementParser)
                from sc in Parse.Char(';')
                select r
            ).Named("Statement List");

        /// <summary>
        /// Parse the whole module (a single file, basically).
        /// </summary>
        public static readonly Parser<IStatement[]> ModuleParser =
        (
            from statements in StatementParser.Many()
            from ws in Parse.WhiteSpace.Many()
            select statements.ToArray()
        ).Named("Module");
    }
}
