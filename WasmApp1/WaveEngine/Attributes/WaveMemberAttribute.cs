// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Collections.Generic;
using System.Text;

namespace WaveEngine.Common.Attributes
{
    /// <summary>
    /// Attribute that represents a member with specific settings.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class WaveMemberAttribute : WaveAttributeBase
    {
        // TODO: Handle custom name
        ////public string Name { get; }

        /// <summary>
        /// Gets the order of the member.
        /// </summary>
        public int? Order { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaveMemberAttribute"/> class.
        /// </summary>
        public WaveMemberAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaveMemberAttribute"/> class.
        /// </summary>
        /// <param name="order">The member order in the serialized structure.</param>
        public WaveMemberAttribute(int order)
        {
            this.Order = order;
        }

        ////public WaveMemberAttribute(string name)
        ////{
        ////    this.Name = name;
        ////}

        ////public WaveMemberAttribute(int order, string name)
        ////{
        ////    this.Order = order;
        ////    this.Name = name;
        ////}
    }
}
