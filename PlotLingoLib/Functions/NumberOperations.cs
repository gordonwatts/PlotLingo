using System.ComponentModel.Composition;

namespace PlotLingoLib.Functions
{
    /// <summary>
    /// Contains operations for basic number types (integer and double)
    /// </summary>
    [Export(typeof(IFunctionObject))]
    class NumberOperations : IFunctionObject
    {
        public static int OperatorPlus(int o1, int o2)
        {
            return o1 + o2;
        }

        public static int OperatorMinus(int o1, int o2)
        {
            return o1 - o2;
        }

        public static int OperatorMultiply(int o1, int o2)
        {
            return o1 * o2;
        }

        public static double OperatorDivide(int o1, int o2)
        {
            return o1 / ((double)o2);
        }

        public static double OperatorPlus(double o1, double o2)
        {
            return o1 + o2;
        }

        public static double OperatorMinus(double o1, double o2)
        {
            return o1 - o2;
        }

        public static double OperatorMultiply(double o1, double o2)
        {
            return o1 * o2;
        }

        public static double OperatorDivide(double o1, double o2)
        {
            return o1 / o2;
        }
    }
}
