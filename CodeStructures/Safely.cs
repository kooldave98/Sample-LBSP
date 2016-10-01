using System;

namespace CodeStructures
{
    public static class Safely
    {
        public static IMaybe<T> Do<T>(Func<T> to_do) where T : class
        {
            T payload = default(T);

            try
            {
                payload = to_do();
            }
            catch (Exception e)
            {
                //record_it(e);
            }

            return payload.to_maybe();
        }
    }
}
