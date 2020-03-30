// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Reflection;

namespace WaveEngine.Framework.Dependencies
{
    /// <summary>
    /// Set Values.
    /// </summary>
    internal class FieldAccessor : ValueAccessor
    {
        private FieldInfo fieldInfo;

        public override string MemberName => this.fieldInfo.Name;

        public override Type Type => this.fieldInfo.FieldType;

        public FieldAccessor(FieldInfo fieldInfo)
        {
            this.fieldInfo = fieldInfo;
        }

        public override object GetValue(DependencyObject instance)
        {
            return this.fieldInfo.GetValue(instance);
        }

        public override void SetValue(DependencyObject instance, object newValue)
        {
            this.fieldInfo.SetValue(instance, newValue);
        }
    }
}
