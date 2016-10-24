using System;
using System.Runtime.CompilerServices;

namespace CodeStructures
{
    public class Guard
    {
        public static T IsNotNull<T>(T input, string parameter)
        {
            if(input == null)
            {
                throw new ArgumentNullException($"{parameter} cannot be null");
            }

            return input;
        }
    }
}
