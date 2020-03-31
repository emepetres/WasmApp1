// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;

namespace WaveEngine.Framework.Dependencies
{
    /// <summary>
    /// Set Values.
    /// </summary>
    internal class ValueResolver : Resolver
    {
        protected readonly Type Type;

        public ValueResolver(Type type, BindObject bindAttribute, ValueAccessor valueAccessor)
            : base(bindAttribute, valueAccessor)
        {
            this.Type = type;
        }

        public override object ResolveObject(DependencyObject instance)
        {
            object resolvedValue = null;

            var obj = this.BindAttribute.Resolve(instance, this.Type);

            if (obj != null)
            {
                // Create a dependency link if the object is DependencyObject
                if (obj is DependencyObject dependency)
                {
                    var link = new DependencyLink(instance, dependency, this);
                    link.Register();
                }

                resolvedValue = obj;
            }

            return resolvedValue;
        }

        public override void Delete(DependencyObject instance, DependencyObject instanceToDelete, bool checkRequired)
        {
            if (checkRequired && this.BindAttribute.IsRequired)
            {
                instance.DependencyBroken();
            }

            this.ValueAccessor.SetValue(instance, null);
        }
    }
}
