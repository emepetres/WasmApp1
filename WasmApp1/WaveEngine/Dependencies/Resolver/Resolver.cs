// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

namespace WaveEngine.Framework.Dependencies
{
    /// <summary>
    /// Set Values.
    /// </summary>
    public abstract class Resolver
    {
        internal readonly BindObject BindAttribute;

        internal ValueAccessor ValueAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="Resolver"/> class.
        /// </summary>
        /// <param name="bindAttribute">The bind attribute.</param>
        /// <param name="valueAccessor">The value accessor.</param>
        public Resolver(BindObject bindAttribute, ValueAccessor valueAccessor)
        {
            this.ValueAccessor = valueAccessor;
            this.BindAttribute = bindAttribute;
        }

        /// <summary>
        /// Resolves the value of specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns><c>true</c> if the value had been resolved or th bind is not required; otherwise, returns false.</returns>
        public bool Resolve(DependencyObject instance)
        {
            // Resolve the object
            var resolvedValue = this.ResolveObject(instance);

            // Sets the new value
            if (resolvedValue != null)
            {
                this.ValueAccessor.SetValue(instance, resolvedValue);
                return true;
            }
            else
            {
                return !this.BindAttribute.IsRequired;
            }
        }

        /// <summary>
        /// Resolves the value of specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>The resolved value.</returns>
        public abstract object ResolveObject(DependencyObject instance);

        /// <summary>
        /// Deletes the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="instanceToDelete">The instance to delete.</param>
        /// <param name="checkRequired">if set to <c>true</c> [check required].</param>
        public abstract void Delete(DependencyObject instance, DependencyObject instanceToDelete, bool checkRequired);
    }
}
