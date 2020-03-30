// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Linq;
using System.Reflection;

namespace WaveEngine.Framework.Dependencies
{
    /// <summary>
    /// Set Values.
    /// </summary>
    internal class CollectionResolver : Resolver
    {
        private readonly Type listType;
        private readonly Type genericArgumentType;

        private readonly MethodInfo addMethod;
        private readonly MethodInfo removeMethod;

        public CollectionResolver(Type listType, BindObject bindAttribute, ValueAccessor valueAccessor)
            : base(bindAttribute, valueAccessor)
        {
            this.listType = listType;

            var genericTypes = this.listType.GetGenericArguments();

            if (genericTypes?.Length == 1)
            {
                this.genericArgumentType = genericTypes.FirstOrDefault();
            }

            this.addMethod = this.listType.GetMethod("Add");
            this.removeMethod = this.listType.GetMethod("Remove");
        }

        public override object ResolveObject(DependencyObject instance)
        {
            object resolvedValue = null;

            var collection = this.BindAttribute.ResolveCollection(instance, this.genericArgumentType);
            if (collection != null)
            {
                var newList = Activator.CreateInstance(this.listType);

                foreach (var item in collection)
                {
                    // Create a dependency link if the object is DependencyObject
                    if (item is DependencyObject dependency)
                    {
                        var link = new DependencyLink(instance, dependency, this);
                        link.Register();
                    }

                    this.addMethod.Invoke(newList, new object[] { item });
                }

                resolvedValue = newList;
            }

            return resolvedValue;
        }

        public override void Delete(DependencyObject instance, DependencyObject instanceToDelete, bool checkRequired)
        {
            var currentList = this.ValueAccessor.GetValue(instance);
            if (currentList != null)
            {
                this.removeMethod.Invoke(currentList, new object[] { instanceToDelete });
            }
        }
    }
}
