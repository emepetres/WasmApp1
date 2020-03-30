// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Collections.Generic;
using WaveEngine.Common.Dependencies;

namespace WaveEngine.Common
{
    /// <summary>
    /// Interface that represents an object that has dependencies defined.
    /// </summary>
    public interface IDependencyObject
    {
        /// <summary>
        /// Gets the dependency list.
        /// </summary>
        Lazy<List<IDependencyLink>> Dependencies { get; }
    }
}
