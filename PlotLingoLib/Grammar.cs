using PlotLingoLib.Expressions;
using PlotLingoLib.Expressions.Values;
using PlotLingoLib.Statements;
using Sprache;
using System;
using System.Collections.Generic;
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
                .DelimitedBy(Parse.Char(',')).Optional()
                .Contained(Parse.Char('['), Parse.Char(']'))
            select new ArrayValue(values.IsEmpty ? new IExpression[]{} : values.Get().ToArray());

        /// <summary>
        /// Our basic identifier, standard.
        /// </summary>
        private static readonly Parser<string> IdentifierParser = (Parse.LetterOrDigit.Or(Parse.Char('_'))).AtLeastOnce().Text().Token().Named("Identifier");

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
            from values in Parse
                .Ref(() => ExpressionParser)
                .DelimitedBy(Parse.Char(',')).Optional()
                .Contained(Parse.Char('('), Parse.Char(')'))
            select values.IsEmpty ? new IExpression[] {} : values.Get().ToArray();

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
        /// Parse an expression surrounded by a "(".
        /// </summary>
        private static readonly Parser<IExpression> GroupedExpressionParser =
            from e in Parse
                .Ref(() => ExpressionParser)
                .Contained(Parse.Char('('), Parse.Char(')'))
            select e;

        /// <summary>
        /// Parse the term for an expression
        /// </summary>
        private static readonly Parser<IExpression> TermParser =
            from t in FunctionExpressionParser
                .Or(ValueExpressionParser)
                .Or(ArrayValueParser)
                .Or(VariableValueParser)
                .Or(GroupedExpressionParser)
            select t;

        /// <summary>
        /// Find an operator/term pairing
        /// </summary>
        private static readonly Parser<Tuple<string, IExpression>> OperatorTermParser =
            from op in Parse.Char('+').Or(Parse.Char('-')).Or(Parse.Char('*')).Or(Parse.Char('/'))
            from t in TermParser
            select Tuple.Create(op.ToString(), t);

        /// <summary>
        /// Top level expression parser
        /// </summary>
        private static readonly Parser<IExpression> ExpressionSubParser =
            from t in TermParser
            from rest in OperatorTermParser.Many()
            select BuildExpressionTree(t, rest);

        /// <summary>
        /// Parse the full expression at the top level. So we deal with things
        /// like method invokation.
        /// </summary>
        private static readonly Parser<IExpression> ExpressionParser =
            from e1 in ExpressionSubParser
            from alist in Parse.Char('.').Then(_ => FunctionExpressionParser).Optional()
            select BuildMethodOrExpression(e1, alist);

        /// <summary>
        /// Build a method expression or regular expression.
        /// </summary>
        /// <param name="e1"></param>
        /// <param name="alist"></param>
        /// <returns></returns>
        private static IExpression BuildMethodOrExpression(IExpression e1, IOption<FunctionExpression> alist)
        {
            if (alist.IsEmpty)
                return e1;

            return new MethodCallExpression(e1, alist.Get());
        }

        /// <summary>
        /// Build an expression from operators. * and / take precedence over +,-.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="rest"></param>
        /// <returns></returns>
        private static IExpression BuildExpressionTree(IExpression t, IEnumerable<Tuple<string, IExpression>> rest)
        {
            // Take care of the simple case.
            List<Tuple<string, IExpression>> lst = new List<Tuple<string, IExpression>>() { Tuple.Create("", t) };
            lst.AddRange(rest);
            if (lst.Count == 1)
                return t;

            // Scan the list, looking for the high precedence operators and replacing them with a new expression
            int index = 0;
            while (index < lst.Count)
            {
                var item = lst[index];
                if (item.Item1 == "/" || item.Item1 == "*")
                {
                    var itemLast = lst[index - 1];
                    var op = new FunctionExpression(item.Item1.ToString(), new IExpression[] { itemLast.Item2, item.Item2 });
                    lst[index - 1] = new Tuple<string, IExpression>(itemLast.Item1, op);
                    lst.Remove(item);
                    index = index - 1;
                }
                else
                {
                    index = index + 1;
                }
            }

            // Next, just combine them all

            var lastexpr = lst[0].Item2;
            foreach (var item in lst.Skip(1))
            {
                lastexpr = new FunctionExpression(item.Item1, new IExpression[] { lastexpr, item.Item2 });
            }

            return lastexpr;
        }

        
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
