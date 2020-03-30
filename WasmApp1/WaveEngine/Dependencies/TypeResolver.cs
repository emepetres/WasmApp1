// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WaveEngine.Common.Helpers;

namespace WaveEngine.Framework.Dependencies
{
    /// <summary>
    /// Dependencies resolved for this type.
    /// </summary>
    internal class TypeResolver
    {
        /// <summary>
        /// The associated type.
        /// </summary>
        private readonly Type type;

        /// <summary>
        /// The bind resolvers of this type.
        /// </summary>
        private readonly Resolver[] bindResolvers;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeResolver"/> class.
        /// By default, this instance is marked as active.
        /// </summary>
        /// <param name="type">The associated type.</param>
        public TypeResolver(Type type)
        {
            this.type = type;

            // Gets bind resolvers of all properties and fields
            this.bindResolvers = this.type.GetTypeInfo().GetAllProperties()
                    .Where(p => p.IsDefined(typeof(BindObject), true))
                    .Select(p => this.ProcessProperty(p))
                .Union(
                    this.type.GetTypeInfo().GetAllFields()
                    .Where(f => f.IsDefined(typeof(BindObject), true))
                    .Select(f => this.ProcessField(f)))
                .ToArray();
        }

        /// <summary>
        /// Resolve the dependencies of an instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>If all dependencies has been satisfied.</returns>
        public bool ResolveDependencies(DependencyObject instance)
        {
            bool isOk = true;
            for (int i = 0; i < this.bindResolvers.Length; i++)
            {
                isOk &= this.bindResolvers[i].Resolve(instance);
            }

            return isOk;
        }

        /// <summary>
        /// Process a property to generate its resolver.
        /// </summary>
        /// <param name="propertyInfo">The property.</param>
        /// <returns>The bind resolver associated to this property.</returns>
        private Resolver ProcessProperty(PropertyInfo propertyInfo)
        {
            var propertyAccessor = new PropertyAccessor(propertyInfo);
            var bindAttribute = propertyInfo.GetCustomAttribute<BindObject>(true);
            return this.CreateResolver(propertyAccessor, bindAttribute);
        }

        /// <summary>
        /// Process a field to generate its resolver.
        /// </summary>
        /// <param name="fieldInfo">The field.</param>
        /// <returns>The bind resolver associated to this field.</returns>
        private Resolver ProcessField(FieldInfo fieldInfo)
        {
            var fieldAccessor = new FieldAccessor(fieldInfo);
            var bindAttribute = fieldInfo.GetCustomAttribute<BindObject>(true);
            return this.CreateResolver(fieldAccessor, bindAttribute);
        }

        private Resolver CreateResolver(ValueAccessor accessor, BindObject bindAttribute)
        {
            var type = accessor.Type;

            var isList = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
            if (!isList)
            {
                return new ValueResolver(type, bindAttribute, accessor);
            }
            else
            {
                return new CollectionResolver(type, bindAttribute, accessor);
            }
        }
    }
}
