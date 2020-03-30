// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using WaveEngine.Framework.Services;

namespace WaveEngine.Framework.Exceptions
{
    /// <summary>
    /// This class represent an exception related to an Scene Behavior.
    /// </summary>
    public class ServiceException : WaveException
    {
        /// <summary>
        /// Gets the service type.
        /// </summary>
        public Type ServiceType
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceException" /> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="innerException">The inner exception.</param>
        internal ServiceException(Service service, Exception innerException)
            : base(CreateMessage(service.GetType(), innerException), innerException)
        {
            this.ServiceType = service.GetType();
        }

        /// <summary>
        /// Create the exception message.
        /// </summary>
        /// <param name="type">The service type.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <returns>The exception message.</returns>
        private static string CreateMessage(Type type, Exception innerException)
        {
            return string.Format("Service exception \"{0}\": {1}", type.Name, innerException.Message);
        }
    }
}
