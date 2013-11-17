using PlotLingoLib.Expressions;
using PlotLingoLib.Expressions.Functions;
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
        /// Our basic identifier, standard.
        /// </summary>
        private static readonly Parser<string> IdentifierParser = (Parse.LetterOrDigit.Or(Parse.Char('_')).Or(Parse.Char('-'))).AtLeastOnce().Text().Token().Named("Identifier");

        /// <summary>
        /// A variable name can be any identifier.
        /// </summary>
        private static readonly Parser<string> VariableNameParser = IdentifierParser;

        /// <summary>
        /// Assignment. This will create or replace a current item.
        /// </summary>
        internal class AssignmentStatement : IStatement
        {
            private string nv;
            private IExpression expr;

            public AssignmentStatement(string nv, IExpression expr)
            {
                // TODO: Complete member initialization
                this.nv = nv;
                this.expr = expr;
            }


            public void Evaluate(Context c)
            {
                c.SetVariableValue(nv, expr.Evaluate(c));
            }
        }

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
        /// Parse a value (like a number or a string).
        /// </summary>
        private static readonly Parser<IExpression> ValueExpressionParser =
            (
            from v in StringValueParser
            select v
            );

        /// <summary>
        /// Represents a string value.
        /// </summary>
        private class StringValue : IExpression
        {
            private string content;

            public StringValue(string content)
            {
                // TODO: Complete member initialization
                this.content = content;
            }


            public object Evaluate(Context c)
            {
                return content;
            }
        }

        /// <summary>
        /// A function expression.
        /// </summary>
        internal class FunctionExpression : IExpression
        {
            private string fname;
            private IExpression[] args;

            public FunctionExpression(string fname, IExpression[] args)
            {
                // TODO: Complete member initialization
                this.fname = fname;
                this.args = args;
            }


            public object Evaluate(Context c)
            {
                if (fname == "file")
                {
                    return File.Execute(args.Select(e => e.Evaluate(c)).ToArray());
                }
                throw new System.NotImplementedException(string.Format("Unknown function '{0}' referenced!", fname));
            }
        }

        /// <summary>
        /// Parser that returns a function.
        /// </summary>
        private static readonly Parser<IExpression> FunctionExpressionParser =
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
            from arg1 in ExpressionParser
            from rest in Parse.Char(',').Then(_ => ExpressionParser).Many()
            from closep in Parse.Char(')')
            select new IExpression[] { arg1 }.Concat(rest).ToArray()
            );

        /// <summary>
        /// Parse an expression. Could be a function, or... etc.
        /// </summary>
        private static readonly Parser<IExpression> ExpressionParser =
            (
            from e in FunctionExpressionParser.Or(ValueExpressionParser)
            select e
            );

        /// <summary>
        /// Parse an assignment statement.
        /// </summary>
        private static readonly Parser<IStatement> AssignmentStatementParser =
            (
                from nv in VariableNameParser
                from eq in Parse.Char('=')
                from expr in ExpressionParser
                select new AssignmentStatement(nv, expr)
            ).Named("Assignment Statement");

        /// <summary>
        /// An expression statement
        /// </summary>
        internal class ExpressionStatement : IStatement
        {
            /// <summary>
            /// Expression this statement represents
            /// </summary>
            private IExpression _expr;

            public ExpressionStatement(IExpression expr)
            {
                this._expr = expr;
            }

            /// <summary>
            /// We just evaluate the expression.
            /// </summary>
            /// <param name="c"></param>
            public void Evaluate(Context c)
            {
                _expr.Evaluate(c);
            }
        }

        /// <summary>
        /// Parse an expression statement
        /// </summary>
        private static readonly Parser<IStatement> ExpressionStatementParser =
            (
                from expr in ExpressionParser
                select new ExpressionStatement(expr)
            ).Named("Expression Statement");

        /// <summary>
        /// Parse a statement
        /// </summary>
        private static readonly Parser<IStatement> StatementParser =
            (
                from r in AssignmentStatementParser.Or(ExpressionStatementParser)
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
