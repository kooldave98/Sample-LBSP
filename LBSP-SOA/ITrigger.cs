using System;
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

    public class TriggerInitialisationError<T> : IErrorTrigger where T : ITrigger
    {
    }

    public class UnknownError : IErrorTrigger
    {
    }

    public static class TriggerExtensions
    {
        public static RawEvent ToRawEvent(this ITrigger trigger, Guid? parent_id = null)
        {
            return
                new RawEvent(Guid.NewGuid()
                                , trigger.ToBytes()
                                , new { parent_id }.ToBytes()//Every event has a pointer to its parent event
                                , trigger.GetType().Name);
        }

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
