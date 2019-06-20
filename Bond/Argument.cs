using System;

namespace Bond
{
    internal class Argument
    {
        internal static void IsNotNull<T>(T argument, string argumentName) where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }
    }
}
