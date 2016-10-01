using System;
using System.Collections.Generic;

namespace CodeStructures
{

    /// <summary>
    ///     Maybe monad
    /// </summary>
    /// <typeparam name="T">
    ///     The underlying type that the maybe maps
    /// </typeparam>
    public interface IMaybe<T> { }


    public class Nothing { }

    /// <summary>
    ///     The nothing variant of a maybe.  This represents a 
    ///  error state.
    /// </summary>
    /// <typeparam name="T">
    ///     The underllying type that the maybe maps
    /// </typeparam>
    public class Nothing<T>
        : Nothing
    , IMaybe<T>
    { }

    /// <summary>
    ///     The has a value variant of a maybe.
    /// </summary>
    /// <typeparam name="T">
    ///     The underllying type that the maybe maps
    /// </typeparam>
    public class Just<T>
        : IMaybe<T>
    {

        /// <summary>
        ///     The value of the underlying type.
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        ///     Constructor requires an instance of the underlying type.
        /// </summary>
        /// <param name="value">
        ///     The value for the 
        /// </param>
        public Just
        (T value)
        {
            Value = value;
        }


        public static implicit operator T(Just<T> to_convert)
        {

            return to_convert.Value;
        }
    }

    public static class MaybeExtensions
    {

        public static IMaybe<Q> bind<P, Q>
        (this IMaybe<P> subject
            , Func<P, IMaybe<Q>> transform)
        {

            return subject.Match(

                has_value: transform,

                nothing: () => new Nothing<Q>()
            );

        }

        public static IMaybe<T> apply<T>
        (this IMaybe<T> apply_to
            , Func<T, IMaybe<T>> function)
        {

            if (apply_to == null)
            {
                return new Nothing<T>();
            }

            if (apply_to is Nothing<T>)
            {
                return apply_to;
            }

            if (apply_to is Just<T>)
            {
                return function(((Just<T>)apply_to).Value);
            }

            throw new Exception("Unhandled situation");
        }


        /// <summary>
        ///     A discriminated union for the maybe monad
        /// </summary>
        /// <typeparam name="T">
        ///     The underlying type that the monad represents
        /// </typeparam>
        /// <param name="maybe">
        ///     The instance of the maybe that the match will be performed on.
        /// </param>
        /// <param name="has_value">
        ///     Delegate that is executed if the monad has a value.
        /// </param>
        /// <param name="nothing">
        ///     Delegate that is executed if the monad does not have a value
        /// </param>
        public static void Match<T>
        (this IMaybe<T> maybe
            , Action<T> has_value
            , Action nothing)
        {

            maybe
                .Match(

                    has_value:
                    value => { has_value(value); return new Unit(); },

                    nothing:
                    () => { nothing(); return new Unit(); }

                );
        }

        public static Q Match<P, Q>
        (this IMaybe<P> maybe
            , Func<P, Q> has_value
            , Func<Q> nothing)
        {

            if (maybe is Just<P>)
            {
                var value = (Just<P>)maybe;
                return has_value(value.Value);

            }

            if (maybe is Nothing<P>)
            {
                return nothing();
            }

            throw new Exception("Unmatched case");
        }

        public static P GetValueOrDefault<P>(this IMaybe<P> maybe) where P : class
        {
            if (maybe is Just<P>)
            {
                var value = (Just<P>)maybe;
                return value.Value;
            }

            if (maybe is Nothing<P>)
            {
                return default(P);
            }

            throw new Exception("Unmatched case");
        }

        public static P GetValueOrDefault<P>(this IMaybe<P> maybe
                                            , P default_value)
        {
            if (maybe is Just<P>)
            {
                var value = (Just<P>)maybe;
                return value.Value;
            }

            if (maybe is Nothing<P>)
            {
                return default_value;
            }

            throw new Exception("Unmatched case");
        }

        /// <summary>
        ///     Check whether a maybe outcome has been decided
        /// </summary>
        /// <param name="maybe">
        ///     The result of the date validation that the check will
        ///  be performed against.
        /// </param>
        /// <returns>
        ///     True if the maybe is not null
        /// </returns>
        public static bool has_been_decided<T>
        (this IMaybe<T> maybe)
        {

            return maybe != null;
        }

        public static IEnumerable<T> SelectJustValues<T>
        (this IEnumerable<IMaybe<T>> source)
        {

            foreach (var entry in source)
            {
                T next_value = default(T);
                var has_value = false;

                entry.Match(

                    has_value: value =>
                    {
                        has_value = true;
                        next_value = value;
                    },

                    nothing: () =>
                    {
                        has_value = false;
                    }
                );

                if (has_value)
                {
                    yield return next_value;
                }
            }



        }

        public static IMaybe<T> to_maybe<T>
        (this T source)
            where T : class
        {

            return source != null
                ? new Just<T>(source) as IMaybe<T>
                    : new Nothing<T>();
        }

    }

    public sealed class Unit
    {

        public Unit() { }

    }
}