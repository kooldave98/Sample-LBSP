using System.Collections.Generic;

namespace CodeStructures
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this T input)
        {
            yield return input;
        }
    }
}
