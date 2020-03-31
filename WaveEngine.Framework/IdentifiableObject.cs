// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using WaveEngine.Common.Attributes;

namespace WaveEngine.Common
{
    /// <summary>
    /// Abstract class to extend for each class that we need to serialize.
    /// </summary>
    public abstract class IdentifiableObject
    {
        private Guid id;

        /// <summary>
        /// Gets or sets the Id of this object.
        /// </summary>
        [WaveMember(0)]
        public Guid Id
        {
            get
            {
                return this.id;
            }

            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifiableObject"/> class.
        /// </summary>
        public IdentifiableObject()
        {
            // Auto-generate the Id of this object.
            this.Id = Guid.NewGuid();
        }
    }
}
