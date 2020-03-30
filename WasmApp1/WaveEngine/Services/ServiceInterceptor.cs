// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WaveEngine.Framework.Services
{
    /// <summary>
    /// Manages <see cref="Service"/> instances from registered in the <see cref="Application.Container"/>.
    /// </summary>
    /// <remarks>
    /// Registered <see cref="Service"/> instances that inherit from <see cref="UpdatableService"/>
    /// will be updated automatically by this instance. This update order depends on the order in which
    /// <see cref="Service"/> instances where registered (first registered, first to be updated and so on).
    /// </remarks>
    public class ServiceInterceptor : ContainerInterceptor<Service>
    {
        /// <summary>
        /// Registered <see cref="Service"/> instances.
        /// </summary>
        private readonly Dictionary<Type, Service> services;

        /// <summary>
        /// Registered <see cref="Service"/> instances.
        /// </summary>
        private readonly LinkedList<Service> serviceLinkedList;

        /// <summary>
        /// Registered <see cref="UpdatableService"/> instances that require to be updated every frame.
        /// </summary>
        private readonly List<UpdatableService> updatableServices;

        /// <summary>
        /// The services have been initialized.
        /// </summary>
        private bool isInitialized;

        private object lockProcessing = new object();

        private AttachableObjectState state = AttachableObjectState.Detached;
        private bool isStarted = false;

        /// <summary>
        /// Gets the number of active services.
        /// </summary>
        public int ActiveServicesCount => this.services.Count;

        /// <summary>
        /// Gets a value indicating whether this object is activated.
        /// </summary>
        public virtual bool IsAttached => (this.state == AttachableObjectState.Deactivated) || (this.state == AttachableObjectState.Activated);

        /// <summary>
        /// Gets a value indicating whether this object is started.
        /// </summary>
        public virtual bool IsActivated => this.state == AttachableObjectState.Activated;

        /// <summary>
        /// Gets a value indicating whether this object is started.
        /// </summary>
        public virtual bool IsStarted => this.isStarted && this.state == AttachableObjectState.Activated;

        /// <summary>
        /// Gets a value indicating whether this object is destroyed.
        /// </summary>
        public virtual bool IsDestroyed => this.state == AttachableObjectState.Destroyed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceInterceptor"/> class.
        /// </summary>
        public ServiceInterceptor()
        {
            this.services = new Dictionary<Type, Service>();
            this.serviceLinkedList = new LinkedList<Service>();
            this.updatableServices = new List<UpdatableService>();
        }

        /// <summary>
        /// Initializes the registered services.
        /// </summary>
        /// <remarks>
        /// This method initialize only new services, not initialized yet.
        /// </remarks>
        public void InitializeServices()
        {
            if (this.isInitialized)
            {
                return;
            }

            // Attach services.
            var current = this.serviceLinkedList.First;
            while (current != null)
            {
                var service = current.Value;

                try
                {
                    service.Attach();
                }
                catch (Exception ex)
                {
                    if (this.CaptureServiceException(service, ex))
                    {
                        throw;
                    }
                }

                current = current.Next;
            }

            this.state = AttachableObjectState.Deactivated;

            // Activate services.
            current = this.serviceLinkedList.First;
            while (current != null)
            {
                var service = current.Value;

                try
                {
                    if (service.IsAttached)
                    {
                        service.Activate();
                    }
                }
                catch (Exception ex)
                {
                    if (this.CaptureServiceException(service, ex))
                    {
                        throw;
                    }
                }

                current = current.Next;
            }

            this.state = AttachableObjectState.Activated;

            // Start services.
            current = this.serviceLinkedList.First;
            while (current != null)
            {
                var service = current.Value;

                try
                {
                    if (service.IsActivated)
                    {
                        service.BaseStart();
                    }
                }
                catch (Exception ex)
                {
                    if (this.CaptureServiceException(service, ex))
                    {
                        throw;
                    }
                }

                current = current.Next;
            }

            this.isInitialized = true;
        }

        /// <summary>
        /// Called when [deactivated].
        /// </summary>
        internal void OnActivated()
        {
            foreach (Service service in this.services.Values)
            {
                try
                {
                    service.Activate();
                }
                catch (Exception ex)
                {
                    if (this.CaptureServiceException(service, ex))
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        internal void OnDeactivated()
        {
            foreach (Service service in this.services.Values)
            {
                try
                {
                    service.ForceState(AttachableObjectState.Deactivated);
                }
                catch (Exception ex)
                {
                    if (this.CaptureServiceException(service, ex))
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        internal void OnDetach()
        {
            foreach (Service service in this.services.Values)
            {
                try
                {
                    service.ForceState(AttachableObjectState.Detached);
                }
                catch (Exception ex)
                {
                    if (this.CaptureServiceException(service, ex))
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        internal void OnDestroy()
        {
            foreach (Service service in this.services.Values)
            {
                try
                {
                    service.ForceState(AttachableObjectState.Destroyed);
                }
                catch (Exception ex)
                {
                    if (this.CaptureServiceException(service, ex))
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the registered <see cref="UpdatableService"/> instances.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void UpdateServices(TimeSpan gameTime)
        {
            for (int i = 0; i < this.updatableServices.Count; i++)
            {
                var updatableService = this.updatableServices[i];

                try
                {
                    if (updatableService.IsActivated)
                    {
                        updatableService.Update(gameTime);
                    }
                }
                catch (Exception ex)
                {
                    if (this.CaptureServiceException(updatableService, ex))
                    {
                        throw;
                    }
                }
            }
        }

        /// <inheritdoc />
        public override void OnRegistered(Type type)
        {
        }

        /// <inheritdoc />
        public override void OnUnregistered(Type type)
        {
            if (this.services.TryGetValue(type, out var service))
            {
                this.services.Remove(type);
                this.RemoveService(service, true);
            }
        }

        /// <inheritdoc />
        protected override void OnInstanced(Service instance)
        {
            this.AddService(instance);
        }

        /// <summary>
        /// Registers a <see cref="Service"/> instance.
        /// </summary>
        /// <param name="service">The instance to register.</param>
        /// <remarks>
        /// If the instance to register inherits from <see cref="UpdatableService"/> this class
        /// will handle the update automatically.
        /// </remarks>
        /// <exception cref="ArgumentNullException">If service is null.</exception>
        /// <exception cref="ArgumentException">If service is disposed or already attached.</exception>
        /// <exception cref="InvalidOperationException">If a <see cref="Service"/> with the same type was already registered.</exception>
        private void AddService(Service service)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            var serviceType = service.GetType();

            // The service must be detached
            if (service.IsAttached)
            {
                throw new ArgumentException($"The service instance is already attached (\"{serviceType}\")");
            }

            // The service has been disposed
            if (service.IsDestroyed)
            {
                throw new ArgumentException($"The service instance has been disposed (\"{serviceType}\")");
            }

            // Checks if the service is already registered
            if (this.services.ContainsKey(serviceType))
            {
                Trace.TraceWarning($"There was already a service with the same type (\"{serviceType}\") registered");
            }

            this.services.Add(serviceType, service);
            this.serviceLinkedList.AddLast(service);

            // If the service is updatable, register to the updatable service
            if (service is UpdatableService updatable)
            {
                this.updatableServices.Add(updatable);
            }

            // Load the service
            service.Load();

            if (this.isInitialized)
            {
                try
                {
                    // If the services has been initialized, try to attach and start the service
                    if (this.IsAttached)
                    {
                        bool isAttached = service.Attach();

                        if (isAttached
                            && this.IsActivated
                            && service.IsEnabled)
                        {
                            service.Activate();
                            if (this.isInitialized
                                && service.IsActivated)
                            {
                                service.BaseStart();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (this.CaptureServiceException(service, ex))
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Unregisters a <see cref="Service"/>.
        /// </summary>
        /// <param name="service">The service to unregister.</param>
        /// <param name="destroy">True if the service will be destroyed.</param>
        /// <remarks>The unregistered <see cref="Service"/> will be disposed.</remarks>
        private void RemoveService(Service service, bool destroy)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }

            var serviceType = service.GetType();

            // The service must be detached
            if (this.isInitialized && !service.IsAttached)
            {
                throw new ArgumentException($"The service instance is already detached (\"{serviceType}\")");
            }

            // The cannot be disposed
            if (service.IsDestroyed)
            {
                throw new ArgumentException($"The service instance has been disposed (\"{serviceType}\")");
            }

            if (service is UpdatableService updatable)
            {
                this.updatableServices.Remove(updatable);
            }

            try
            {
                service.ForceState(destroy ? AttachableObjectState.Destroyed : AttachableObjectState.Detached);
            }
            catch (Exception ex)
            {
                if (this.CaptureServiceException(service, ex))
                {
                    throw;
                }
            }

            this.services.Remove(serviceType);
        }

        private bool CaptureServiceException(Service service, Exception ex)
        {
            var errorHandler = this.Container?.Resolve<ErrorHandler>();
            return errorHandler?.CaptureException(new Exceptions.ServiceException(service, ex)) ?? false;
        }
    }
}
