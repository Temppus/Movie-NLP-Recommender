using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieRecommender.Extensions
{
    public static class ExceptionExtensions
    {
        public static void ThrowIfNull(this object obj, string argName)
        {
            if (obj == null)
                throw new ArgumentNullException(argName);
        }

        public static void ThrowIfNegative(this int intArg, string argName)
        {
            if (intArg < 0)
                throw new ArgumentException($"{argName} must be positive number.");
        }

        public static void ThrowIfNegativeOrZero(this int intArg, string argName)
        {
            if (intArg <= 0)
                throw new ArgumentException($"{argName} must be greater than 0.");
        }
    }
}