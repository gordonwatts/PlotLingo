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
        /// The legal characters for an identifier. Basically, everything but a digit.
        /// </summary>
        private static readonly Parser<char> FirstIdentifierCharacters = Parse.Letter.Or(Parse.Char('_'));

        /// <summary>
        /// Our basic identifier, standard.
        /// </summary>
        private static readonly Parser<string> IdentifierParser =
            (from fc in FirstIdentifierCharacters
            from rest in (FirstIdentifierCharacters.Or(Parse.Digit)).Many().Text()
            select fc.ToString() + rest).Token();

        /// <summary>
        /// A variable name can be any identifier.
        /// </summary>
        private static readonly Parser<string> VariableNameParser = IdentifierParser;

        /// <summary>
        /// Take a string token, and parse it with all WS that is around.
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        private static Parser<string> StringToken(string op)
        {
            return Parse.String(op).Token().Text();
        }

        /// <summary>
        /// Single tokens
        /// </summary>
        private static readonly Parser<string> Equal = StringToken("=");
        private static readonly Parser<string> Dot = StringToken(".");
        private static readonly Parser<string> SemiColon = StringToken(";");
        private static readonly Parser<string> Plus = StringToken("+");
        private static readonly Parser<string> Minus = StringToken("-");
        private static readonly Parser<string> Times = StringToken("*");
        private static readonly Parser<string> Divide = StringToken("/");
        private static readonly Parser<string> DictionaryRelation = StringToken(":").Or(StringToken("=>"));
        private static readonly Parser<string> OpenBrace = StringToken("{");
        private static readonly Parser<string> CloseBrace = StringToken("}");

        /// <summary>
        /// Binary operators supported by this simple language.
        /// </summary>
        private static readonly Parser<string> BinaryOperators = Plus.Or(Minus).Or(Times).Or(Divide);

        /// <summary>
        /// Parse a string
        /// </summary>
        private static readonly Parser<IExpression> StringValueParser =
            from ws in Parse.WhiteSpace.Many()
            from openp in Parse.Char('"')
            from content in Parse.CharExcept('"').Many().Text()
            from closep in Parse.Char('"')
            select new StringValue(content);

        /// <summary>
        /// Parse an integer value
        /// </summary>
        private static readonly Parser<IExpression> IntegerValueParser =
            from dg in Parse.Digit.AtLeastOnce().Text().Token()
            select new IntegerValue(Int32.Parse(dg));

        /// <summary>
        /// Parse a double value
        /// </summary>
        private static readonly Parser<IExpression> DoubleValueParser =
            (from d in Dot from n in Parse.Number select new DoubleValue(double.Parse("." + n)))
            .Or(from n1 in Parse.Number from d in Dot from n2 in Parse.Number select new DoubleValue(double.Parse(n1 + "." + n2)))
            .Or(from n in Parse.Number from d in Dot select new DoubleValue(double.Parse(n)))
            ;

        /// <summary>
        /// Parse an identifier that is a variable name.
        /// </summary>
        private static readonly Parser<VariableValue> VariableValueParser =
            from name in VariableNameParser
            select new VariableValue(name);

        /// <summary>
        /// Parse a single relation in a dictionary.
        /// </summary>
        /// <remarks> Note that we want expressions that have no further dictionaries
        /// deep down inside their definitions. So in this environment we use a special version of the
        /// expression parser.</remarks>
        private static readonly Parser<Tuple<IExpression, IExpression>> DictionaryItemParser =
            from name in Parse.Ref(() => ExpressionParserND)
            from rl in DictionaryRelation
            from val in Parse.Ref(() => ExpressionParserND)
            select Tuple.Create(name, val);

        /// <summary>
        /// Parse a dictionary expression
        /// </summary>
        private static readonly Parser<IExpression> DictionaryValueParser =
            from values in DictionaryItemParser
            .DelimitedBy(Parse.Char(',')).Optional()
            .Contained(OpenBrace, CloseBrace)
            select values.IsEmpty ? new DictionaryValue() : new DictionaryValue(values.Get());

        /// <summary>
        /// Parse a value (like a number or a string).
        /// </summary>
        private static readonly Parser<IExpression> ValueExpressionParser
            = DictionaryValueParser
            .Or(StringValueParser)
            .Or(DoubleValueParser)
            .Or(IntegerValueParser);

        /// <summary>
        /// Parse an array of expressions
        /// </summary>
        private static readonly Parser<ArrayValue> ArrayValueParser =
            from values in Parse
                .Ref(() => ExpressionParser)
                .DelimitedBy(Parse.Char(',')).Optional()
                .Contained(Parse.Char('['), Parse.Char(']'))
            select new ArrayValue(values.IsEmpty ? new IExpression[] { } : values.Get().ToArray());

        /// <summary>
        /// Parser that returns a function.
        /// </summary>
        private static readonly Parser<FunctionExpression> FunctionExpressionParser =
            from fname in IdentifierParser
            from args in ArgumentListParser
            select new FunctionExpression(fname, args);

        /// <summary>
        /// Parse an argument list that goes to a function or similar.
        /// </summary>
        private static readonly Parser<IExpression[]> ArgumentListParser =
            from values in Parse
                .Ref(() => ExpressionParser)
                .DelimitedBy(Parse.Char(',')).Optional()
                .Contained(Parse.Char('('), Parse.Char(')'))
            select values.IsEmpty ? new IExpression[] { } : values.Get().ToArray();

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
            from op in BinaryOperators
            from t in TermParser
            select Tuple.Create(op.ToString(), t);

        /// <summary>
        /// Parse the operator and the second term in a binary expression.
        /// </summary>
        /// <param name="firstTerm"></param>
        /// <returns></returns>
        /// <remarks>Must be kept in sync with ParseBinaryOperator</remarks>
        private static Parser<Tuple<string, IExpression>> ParseBinaryOperator =
            from op in BinaryOperators.Or(DictionaryRelation)
            from secondTerm in TermParser
            select Tuple.Create(op, secondTerm);

        /// <summary>
        /// Parse the operator and the second term in a binary expression. A single
        /// dictionary item is not allowed inside.
        /// </summary>
        /// <param name="firstTerm"></param>
        /// <returns></returns>
        /// <remarks>Must be kept in sync with ParseBinaryOperatorND</remarks>
        private static Parser<Tuple<string, IExpression>> ParseBinaryOperatorND =
            from op in BinaryOperators
            from secondTerm in TermParser
            select Tuple.Create(op, secondTerm);

        /// <summary>
        /// Parse the "." for a method invokation and the function-call look-alike for the method call.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static Parser<Tuple<string, IExpression>> ParseMethodInvoke =
            from funcargs in Dot
            from funcCall in FunctionExpressionParser
            select Tuple.Create(funcargs, funcCall as IExpression);

        /// <summary>
        /// Parse the basic expressions, and their operators, one at a go.
        /// </summary>
        /// <remarks>
        /// This parser is written like this because there is no left recursion implemented in Sprache.
        /// So, we know we have some sort of expression, and then an operator. It could be "+" or it could be ".".
        /// We have to alter how we do the parsing depending on which one it is.
        /// Must be kept in sync with ExpressionParserND
        /// </remarks>
        private static readonly Parser<IExpression> ExpressionParser =
            from t in TermParser
            from alist in (ParseBinaryOperator.Or(ParseMethodInvoke)).Many().Optional()
            select BuildExpressionTree(t, alist);

        /// <summary>
        /// Parse the basic expression, and their operators, in on go. Do not allow for a free
        /// standing single dictionary definitions.
        /// </summary>
        /// <remarks>Must be kept in sync with ExpressionParser</remarks>
        private static readonly Parser<IExpression> ExpressionParserND =
            from t in TermParser
            from alist in (ParseBinaryOperatorND.Or(ParseMethodInvoke)).Many().Optional()
            select BuildExpressionTree(t, alist);

        /// <summary>
        /// Build an expression from operators. * and / take precedence over +,-.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="rest"></param>
        /// <returns></returns>
        private static IExpression BuildExpressionTree(IExpression t, IOption<IEnumerable<Tuple<string, IExpression>>> rest)
        {
            // Put everything into a single list for later processing. And take care of the simplest case.
            if (rest.IsEmpty)
                return t;
            List<Tuple<string, IExpression>> lst = new List<Tuple<string, IExpression>>() { Tuple.Create("", t) };
            lst.AddRange(rest.Get());

            // Look for each set of operators in turn.
            CombineForOperators(lst, new string[] { "." }, (obj, opName, funcCall) => new MethodCallExpression(obj, funcCall as FunctionExpression));
            CombineForOperators(lst, new string[] { ":", "=>" }, (key, opName, val) => new DictionaryValue(new Tuple<IExpression, IExpression>[] { Tuple.Create(key, val) }));
            CombineForOperators(lst, new string[] { "*", "/" }, (left, opName, right) => new FunctionExpression(opName, new IExpression[] { left, right }));

            // Next, just combine the left overs. We could use Combine, but then we'd have to add funny logic to it.

            var lastexpr = lst[0].Item2;
            foreach (var item in lst.Skip(1))
            {
                lastexpr = new FunctionExpression(item.Item1, new IExpression[] { lastexpr, item.Item2 });
            }

            return lastexpr;
        }

        /// <summary>
        /// Scan the list for the given operator list. Combine those in the proper way.
        /// </summary>
        /// <param name="lst">The list of operations, one at a time. string is the operator with the previous item.</param>
        /// <param name="combineFunc">Function that takes left and right operands and an operator and returns a new expression</param>
        /// <param name="operators">The list of operators we are going to run the combine on.</param>
        private static void CombineForOperators(List<Tuple<string, IExpression>> lst, string[] operators, Func<IExpression, string, IExpression, IExpression> combineFunc)
        {
            int index = 0;
            while (index < lst.Count)
            {
                var item = lst[index];
                if (operators.Contains(item.Item1))
                {
                    var itemLast = lst[index - 1];
                    var op = combineFunc(itemLast.Item2, item.Item1, item.Item2);
                    lst[index - 1] = new Tuple<string, IExpression>(itemLast.Item1, op);
                    lst.Remove(item);
                    index = index - 1;
                }
                else
                {
                    index = index + 1;
                }
            }
        }

        /// <summary>
        /// Parse an assignment statement.
        /// </summary>
        private static readonly Parser<IStatement> AssignmentStatementParser =
            (
                from nv in VariableNameParser
                from eq in Equal
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
                from sc in SemiColon
                select r
            ).Named("Statement List");

        /// <summary>
        /// Parse the whole module (a single file, basically).
        /// </summary>
        public static readonly Parser<IStatement[]> ModuleParser =
        (
            from ws in Parse.WhiteSpace.Many()
            from statements in StatementParser.Many()
            select statements.ToArray()
        ).Named("Module");
    }
}
