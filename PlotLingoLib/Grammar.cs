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
    static class Grammar
    {
        private static readonly Parser<string> IdentifierParser = (Parse.LetterOrDigit.Or(Parse.Char('_')).Or(Parse.Char('-'))).AtLeastOnce().Text().Token().Named("Identifier");

        private static readonly Parser<string> VariableNameParser = IdentifierParser;

        private class AssignmentStatement : IStatement
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

        private static readonly Parser<IExpression> StringValueParser =
            (
                from openp in Parse.Char('"')
                from content in Parse.CharExcept('"').Many().Text()
                from closep in Parse.Char('"')
                select new StringValue(content)
            );

        private static readonly Parser<IExpression> ValueExpressionParser =
            (
            from v in StringValueParser
            select v
            );

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

        class FunctionExpression : IExpression
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

        private static readonly Parser<IExpression> FunctionExpressionParser =
            (
            from fname in IdentifierParser
            from args in ArgumentListParser
            select new FunctionExpression(fname, args)
            );


        private static readonly Parser<IExpression[]> ArgumentListParser =
            (
            from openp in Parse.Char('(')
            from arg1 in ExpressionParser
            from rest in Parse.Char(',').Then(_ => ExpressionParser).Many()
            from closep in Parse.Char(')')
            select new IExpression[] { arg1 }.Concat(rest).ToArray()
            );

        private static readonly Parser<IExpression> ExpressionParser =
            (
            from e in FunctionExpressionParser.Or(ValueExpressionParser)
            select e
            );

        private static readonly Parser<IStatement> AssignmentStatementParser =
            (
                from nv in VariableNameParser
                from eq in Parse.Char('=')
                from expr in ExpressionParser
                select new AssignmentStatement(nv, expr)
            ).Named("Assignment Statement");

        private static readonly Parser<IStatement> StatementParser =
            (
                from r in AssignmentStatementParser
                select r
            ).Named("Statement List");

        public static readonly Parser<IStatement[]> ModuleParser =
        (
            from statements in StatementParser.Many()
            from ws in Parse.WhiteSpace.Many()
            select statements.ToArray()
        ).Named("Module");

    }
}
