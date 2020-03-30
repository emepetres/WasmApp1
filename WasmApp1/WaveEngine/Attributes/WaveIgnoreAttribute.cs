// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Collections.Generic;
using System.Text;

namespace WaveEngine.Common.Attributes
{
    /// <summary>
    /// Attribute class that represents a member of a class that shouldn't be serialized.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class WaveIgnoreAttribute : WaveAttributeBase
    {
    }
}
