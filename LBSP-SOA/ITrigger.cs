using System.Collections.Generic;
using System.Linq;
using CodeStructures;

namespace LbspSOA
{
    public interface ITrigger
    {
    }



    public interface IErrorTrigger : ITrigger
    {
        
    }


    public static class TriggerExtensions
    {
        public static bool has_trigger<T>(this IEnumerable<ITrigger> triggers)
        {
            Guard.IsNotNull(triggers, nameof(triggers));

            return triggers.OfType<T>().Any();
        }

        public static bool has_errors(this IEnumerable<ITrigger> triggers)
        {
            Guard.IsNotNull(triggers, nameof(triggers));

            return triggers.has_trigger<IErrorTrigger>();
        }
    }
}
