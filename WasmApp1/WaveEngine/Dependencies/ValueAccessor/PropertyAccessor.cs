// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Reflection;

namespace WaveEngine.Framework.Dependencies
{
    /// <summary>
    /// Set Values.
    /// </summary>
    internal class PropertyAccessor : ValueAccessor
    {
        private PropertyInfo propertyInfo;

        public override string MemberName => this.propertyInfo.Name;

        public override Type Type => this.propertyInfo.PropertyType;

        public PropertyAccessor(PropertyInfo propertyInfo)
        {
            this.propertyInfo = propertyInfo;
        }

        public override object GetValue(DependencyObject instance)
        {
            return this.propertyInfo.GetValue(instance);
        }

        public override void SetValue(DependencyObject instance, object newValue)
        {
            this.propertyInfo.SetValue(instance, newValue);
        }
    }
}
