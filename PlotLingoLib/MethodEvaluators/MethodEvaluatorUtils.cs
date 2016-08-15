using System;
using System.Collections.Generic;
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
        public static MethodInfo ArgumentListMatches(this MethodInfo m, Type[] args, Type callingType)
        {
            // If there are less arguments, then it just doesn't matter.
            var pInfo = m.GetParameters();
            if (pInfo.Length < args.Length)
                return null;

            // In order to deal with generic arguments, we have to re-run this whole thing and see what methods come back.
            var allTypes = args;
            if (pInfo.Length > args.Length)
            {
                var extraArgs = pInfo.Skip(args.Length);
                if (extraArgs.Where(a => !a.IsOptional).Any())
                    return null;

                allTypes = args
                    .Concat(pInfo.Skip(args.Length).Select(a => a.ParameterType)).ToArray();
            }

            // Next, lets look at the various types and see if we can make this match
            var matchedAllTypes = allTypes.Zip(pInfo, (given, required) => Tuple.Create(given, required))
                .Select(tpair => tpair.Item1 == tpair.Item2.ParameterType
                                ? tpair.Item1
                                : ConversionType(tpair.Item1, tpair.Item2.ParameterType))
                .ToArray();       

            var method = callingType.GetMethod(m.Name, matchedAllTypes);

            // Return the method. This could be from a generic class that has now been made non-generic.
            return method;
        }

        /// <summary>
        /// Type isn't quit right - but perhaps we can do a conversion?
        /// </summary>
        /// <param name="original"></param>
        /// <param name="desired"></param>
        /// <returns></returns>
        private static Type ConversionType(Type original, Type desired)
        {
            if (original == typeof(int) && desired == typeof(short))
            {
                return typeof(short);
            }
            return original;
        }

        /// <summary>
        /// Return all possible combinations, anti-symmetric, except the diagonal.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="source1"></param>
        /// <param name="source2"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<T1, T1>> AllCombinations<T1>(this T1[] source)
        {
            for (int i = 0; i < source.Length; i++)
            {
                for (int j = 0; j < source.Length; j++)
                {
                    if (i != j)
                    {
                        yield return Tuple.Create(source[i], source[j]);
                    }
                }
            }
        }
    }
}
