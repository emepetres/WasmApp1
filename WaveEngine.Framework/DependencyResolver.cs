// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Collections.Concurrent;

namespace WaveEngine.Framework.Dependencies
{
    /// <summary>
    /// Resolve types dependencies for custom attributes.
    /// </summary>
    public static class DependencyResolver
    {
        /// <summary>
        /// Cached fields by component type.
        /// </summary>
        private static ConcurrentDictionary<Type, TypeResolver> typeResolvers;

        /// <summary>
        /// Initializes static members of the <see cref="DependencyResolver" /> class.
        /// </summary>
        static DependencyResolver()
        {
            typeResolvers = new ConcurrentDictionary<Type, TypeResolver>();
        }

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        /// <param name="instance">The component.</param>
        /// <returns>If all dependencies has been established.</returns>
        public static bool ResolveDependencies(DependencyObject instance)
        {
            var type = instance.GetType();

            if (!typeResolvers.TryGetValue(type, out var typeResolver))
            {
                typeResolver = new TypeResolver(type);
                typeResolvers.TryAdd(type, typeResolver);
            }

            return typeResolver.ResolveDependencies(instance);
        }
    }
}
