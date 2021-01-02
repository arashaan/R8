using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace R8.FileHandlers
{
    internal static class PropertyCopy
    {
        /// <summary>
        /// Copies all public, readable properties from the source object to the
        /// target. The target type does not have to have a parameterless constructor,
        /// as no new instance needs to be created.
        /// </summary>
        /// <remarks>Only the properties of the source and target types themselves
        /// are taken into account, regardless of the actual types of the arguments.</remarks>
        /// <typeparam name="TSource">Type of the source</typeparam>
        /// <typeparam name="TTarget">Type of the target</typeparam>
        /// <param name="source">Source to copy properties from</param>
        /// <param name="target">Target to copy properties to</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void CopyTo<TSource, TTarget>(this TSource source, TTarget target)
            where TSource : class
            where TTarget : class
        {
            PropertyCopier<TSource, TTarget>.Copy(source, target);
        }

        /// <summary>
        /// Copies all public, readable properties from the source object to the
        /// target. The target type does not have to have a parameterless constructor,
        /// as no new instance needs to be created.
        /// </summary>
        /// <remarks>Only the properties of the source and target types themselves
        /// are taken into account, regardless of the actual types of the arguments.</remarks>
        /// <typeparam name="TSource">Type of the source</typeparam>
        /// <typeparam name="TTarget">Type of the target</typeparam>
        /// <param name="source">Source to copy properties from</param>
        /// <param name="target">Target to copy properties to</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Copy<TSource, TTarget>(TSource source, TTarget target)
            where TSource : class
            where TTarget : class
        {
            PropertyCopier<TSource, TTarget>.Copy(source, target);
        }
    }

    /// <summary>
    /// Generic class which copies to its target type from a source
    /// type specified in the Copy method. The types are specified
    /// separately to take advantage of type inference on generic
    /// method arguments.
    /// </summary>
    public static class PropertyCopy<TTarget> where TTarget : class, new()
    {
        /// <summary>
        /// Copies all readable properties from the source to a new instance
        /// of TTarget.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static TTarget CopyFrom<TSource>(TSource source) where TSource : class
        {
            return PropertyCopier<TSource, TTarget>.Copy(source);
        }
    }

    /// <summary>
    /// Static class to efficiently store the compiled delegate which can
    /// do the copying. We need a bit of work to ensure that exceptions are
    /// appropriately propagated, as the exception is generated at type initialization
    /// time, but we wish it to be thrown as an ArgumentException.
    /// Note that this type we do not have a constructor constraint on TTarget, because
    /// we only use the constructor when we use the form which creates a new instance.
    /// </summary>
    internal static class PropertyCopier<TSource, TTarget>
    {
        /// <summary>
        /// Delegate to create a new instance of the target type given an instance of the
        /// source type. This is a single delegate from an expression tree.
        /// </summary>
        private static readonly Func<TSource, TTarget> Creator;

        /// <summary>
        /// List of properties to grab values from. The corresponding targetProperties
        /// list contains the same properties in the target type. Unfortunately we can't
        /// use expression trees to do this, because we basically need a sequence of statements.
        /// We could build a DynamicMethod, but that's significantly more work :) Please mail
        /// me if you really need this...
        /// </summary>
        private static readonly List<PropertyInfo> SourceProperties = new List<PropertyInfo>();

        private static readonly List<PropertyInfo> TargetProperties = new List<PropertyInfo>();
        private static readonly Exception InitializationException;

        /// <summary>
        ///
        /// </summary>
        /// <param name="source"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        internal static TTarget Copy(TSource source)
        {
            if (InitializationException != null)
                throw InitializationException;

            if (source == null)
                throw new ArgumentNullException("source");

            return Creator(source);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <exception cref="ArgumentNullException"></exception>
        internal static void Copy(TSource source, TTarget target)
        {
            if (InitializationException != null)
                throw InitializationException;

            if (source == null)
                throw new ArgumentNullException(nameof(source));

            for (var i = 0; i < SourceProperties.Count; i++)
                TargetProperties[i].SetValue(target, SourceProperties[i].GetValue(source, null), null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        static PropertyCopier()
        {
            try
            {
                Creator = BuildCreator();
                InitializationException = null;
            }
            catch (Exception e)
            {
                Creator = null;
                InitializationException = e;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        private static Func<TSource, TTarget> BuildCreator()
        {
            var sourceParameter = Expression.Parameter(typeof(TSource), "source");
            var bindings = new List<MemberBinding>();
            foreach (var sourceProperty in typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!sourceProperty.CanRead)
                    continue;

                var targetProperty = typeof(TTarget).GetProperty(sourceProperty.Name);
                if (targetProperty == null)
                    throw new ArgumentException("Property " + sourceProperty.Name +
                                                " is not present and accessible in " + typeof(TTarget).FullName);

                if (!targetProperty.CanWrite)
                    throw new ArgumentException("Property " + sourceProperty.Name + " is not writable in " +
                                                typeof(TTarget).FullName);

                if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
                    throw new ArgumentException("Property " + sourceProperty.Name + " is static in " +
                                                typeof(TTarget).FullName);

                if (!targetProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                {
                    throw new ArgumentException("Property " + sourceProperty.Name + " has an incompatible type in " +
                                                typeof(TTarget).FullName);
                }

                bindings.Add(Expression.Bind(targetProperty, Expression.Property(sourceParameter, sourceProperty)));
                SourceProperties.Add(sourceProperty);
                TargetProperties.Add(targetProperty);
            }

            Expression initializer = Expression.MemberInit(Expression.New(typeof(TTarget)), bindings);
            return Expression.Lambda<Func<TSource, TTarget>>(initializer, sourceParameter).Compile();
        }
    }
}