using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlotLingoLib.Expressions.Values;
using PlotLingoLib.Expressions;
using PlotLingoLib;
using System.Collections;
using System.Collections.Generic;

namespace PlotLingoLibTest.Expressions.Values
{
    [TestClass]
    public class DictionaryValueTest
    {
        [TestMethod]
        public void TestGeneration()
        {
            var k1 = new StringValue("1");
            var k2 = new StringValue("2");

            var v1 = new IntegerValue(1);
            var v2 = new IntegerValue(2);

            var allvals = new Tuple<IExpression, IExpression>[] {
                new Tuple<IExpression, IExpression>(k1, v1),
                new Tuple<IExpression, IExpression>(k2, v2),
            };

            var dv = new DictionaryValue(allvals);
            var c = new Context();
            var o = dv.Evaluate(c);

            Assert.IsInstanceOfType(o, typeof(IDictionary<object, object>), "Dict type");
            var od = o as IDictionary<object, object>;
            Assert.AreEqual(1, od["1"], "value of 1");
            Assert.AreEqual(2, od["2"], "value of 2");
        }

        [TestMethod]
        public void TestGenerationNotEvalMoreThanOnce()
        {
            var k1 = new StringValue("1");
            var v1 = new exprEvalOnce();

            var allvals = new Tuple<IExpression, IExpression>[] {
                new Tuple<IExpression, IExpression>(k1, v1),
            };

            var dv = new DictionaryValue(allvals);
            var c = new Context();
            var o = dv.Evaluate(c);
            o = dv.Evaluate(c);

            Assert.AreEqual(1, v1.Evaluated, "# of times evaluated");
        }

        /// <summary>
        /// Record the # of times we get called to evaluate things.
        /// </summary>
        class exprEvalOnce : IExpression
        {
            public exprEvalOnce ()
            {
                Evaluated = 0;
            }

            /// <summary>
            /// Count!
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            public object Evaluate(Context c)
            {
                Evaluated++;
                return 5;
            }

            /// <summary>
            /// Get the # of times we were called to evaluate.
            /// </summary>
            public int Evaluated { get; private set; }
        }
    }
}
