using System;
using System.Linq;
using System.Reflection;

namespace PlotLingoLib.MethodEvaluators
{
    static class MethodEvaluatorUtils
    {
        /// <summary>
        /// Given a method name, see if the argument list is good or not.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool ArgumentListMatches(this MethodInfo m, Type[] args)
        {
            // If there are less arguments, then it just doesn't matter.
            var pInfo = m.GetParameters();
            if (pInfo.Length < args.Length)
                return false;

            // Now, check compatibility of the first set of arguments.
            var commonArgs = args.Zip(pInfo, (margs, pinfo) => Tuple.Create(margs, pinfo.ParameterType));
            if (commonArgs.Where(t => !t.Item1.IsAssignableFrom(t.Item2)).Any())
                return false;

            // And make sure the last set of arguments are actually default!
            return pInfo.Skip(args.Length).All(p => p.IsOptional);
        }
    }
}
