// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using WaveEngine.Common.Helpers;
using IocContainer = DryIoc.Container;

namespace WaveEngine.Framework
{
    /// <summary>
    /// A dependency injection container.
    /// </summary>
    public class Container : IDisposable
    {
        private IocContainer iocContatiner;

        private List<ContainerInterceptor> interceptors;

        /// <summary>
        /// Initializes a new instance of the <see cref="Container"/> class.
        /// </summary>
        public Container()
        {
            this.iocContatiner = new IocContainer(this.ContainerRules);
            this.interceptors = new List<ContainerInterceptor>();
        }

        private Rules ContainerRules(Rules rules)
        {
            rules = rules.WithDefaultReuse(Reuse.Singleton)
                .WithoutThrowOnRegisteringDisposableTransient()
                .With(FactoryMethod.ConstructorWithResolvableArgumentsIncludingNonPublic);

            if (!this.TestDynamicCodeSupport())
            {
                rules = rules.WithoutFastExpressionCompiler();
            }

            return rules;
        }

        private bool TestDynamicCodeSupport()
        {
            try
            {
                var test = new System.Reflection.Emit.DynamicMethod("Test", typeof(void), new Type[0]);
                return true;
            }
            catch (PlatformNotSupportedException)
            {
                return false;
            }
        }

        /// <summary>
        /// Stores the externally created instance into singleton.
        /// </summary>
        /// <typeparam name="TImplementation">The implementation type to be registered.</typeparam>
        /// <param name="instance">The externally created instance.</param>
        public void RegisterInstance<TImplementation>(TImplementation instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            this.CheckNotValidTypes<TImplementation>();
            this.CheckIsRegistered<TImplementation>();

            var implementedTypes = typeof(TImplementation).GetImplementedServiceTypes(true);
            foreach (var type in implementedTypes)
            {
                this.iocContatiner.RegisterDelegate(type, (c) => instance);
            }

            foreach (var interceptor in this.GetInterceptors<TImplementation>())
            {
                interceptor.OnRegistered(typeof(TImplementation));
                interceptor.OnInstanced(instance);
            }
        }

        /// <summary>
        /// Registers service <paramref name="serviceType"/> with corresponding <paramref name="implementationType"/>.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <param name="singleton">Indicates is this types must be registered as singleton.</param>
        public void Register(Type serviceType, Type implementationType, bool singleton = true)
        {
            this.iocContatiner.Register(serviceType, implementationType, singleton ? null : Reuse.Transient);
        }

        /// <summary>
        /// Registers implementation type <typeparamref name="TImplementation"/> for all implemented internal interfaces and base classes.
        /// </summary>
        /// <param name="singleton">Indicates is this types must be registered as singleton.</param>
        /// <typeparam name="TImplementation">The implementation type to be registered.</typeparam>
        public void RegisterType<TImplementation>(bool singleton = true)
        {
            System.Console.WriteLine($"Registering {typeof(TImplementation)}");
            this.CheckNotValidTypes<TImplementation>();
            this.CheckIsRegistered<TImplementation>();

            this.iocContatiner.RegisterMany<TImplementation>(singleton ? null : Reuse.Transient, made: FactoryMethod.ConstructorWithResolvableArgumentsIncludingNonPublic, nonPublicServiceTypes: true);

            System.Console.WriteLine($"Doing interceptors of {typeof(TImplementation)}");
            foreach (var interceptor in this.GetInterceptors<TImplementation>())
            {
                System.Console.WriteLine($"Interceptor: {interceptor}");
                interceptor.OnRegistered(typeof(TImplementation));
                this.iocContatiner.RegisterInitializer<TImplementation>((s, c) => interceptor.OnInstanced(s));
            }
        }

        /// <summary>
        /// Registers a <typeparamref name="T"/> type implemented by <typeparamref name="TImplementation"/> type.
        /// </summary>
        /// <param name="singleton">Indicates is this types must be registered as singleton.</param>
        /// <typeparam name="T">The type to be registered.</typeparam>
        /// <typeparam name="TImplementation">The implementation type to be registered.</typeparam>
        public void RegisterType<T, TImplementation>(bool singleton = true)
            where TImplementation : T
        {
            this.CheckNotValidTypes<TImplementation>();
            this.CheckIsRegistered<TImplementation>();

            this.iocContatiner.Register<T, TImplementation>(singleton ? null : Reuse.Transient);

            foreach (var interceptor in this.GetInterceptors<TImplementation>())
            {
                interceptor.OnRegistered(typeof(TImplementation));
                this.iocContatiner.RegisterInitializer<TImplementation>((s, c) => interceptor.OnInstanced(s));
            }
        }

        /// <summary>
        /// Register a container interceptor.
        /// </summary>
        /// <param name="containerInterceptor">The container interceptor.</param>
        public void RegisterInterceptor(ContainerInterceptor containerInterceptor)
        {
            if (containerInterceptor.Container != null)
            {
                throw new InvalidOperationException("The interceptor is already registered to a container");
            }

            this.interceptors.Add(containerInterceptor);
        }

        /// <summary>Removes specified registration from container.</summary>
        /// <typeparam name="T">The type to be unregistered.</typeparam>
        public void Unregister<T>()
        {
            this.CheckNotValidTypes<T>();

            if (this.iocContatiner.IsRegistered<T>())
            {
                var implementedTypes = typeof(T).GetImplementedServiceTypes(true);
                foreach (var type in implementedTypes)
                {
                    this.iocContatiner.Unregister(type, condition: f => f.ImplementationType == typeof(T));
                }

                foreach (var interceptor in this.GetInterceptors<T>())
                {
                    interceptor.OnUnregistered(typeof(T));
                }
            }
        }

        /// <summary>Removes specified registration from container.</summary>
        /// <param name="instance">The intance to be unregistered.</param>
        /// <typeparam name="T">The type to be unregistered.</typeparam>
        public void Unregister<T>(T instance)
        {
            this.Unregister<T>();
        }

        /// <summary>
        /// Returns true if <typeparamref name = "T" /> is registered in container OR its open generic
        /// definition is registered in container.
        /// </summary>
        /// <typeparam name="T">The type to be check.</typeparam>
        /// <returns>
        /// <c>true</c> if <typeparamref name="T"/> is registered in container OR its open generic definition is
        /// registered in container; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRegistered<T>()
        {
            this.CheckNotValidTypes<T>();
            return this.iocContatiner.IsRegistered<T>();
        }

        /// <summary>
        /// Resolves instance of type <typeparamref name="T"/> from container.
        /// </summary>
        /// <typeparam name="T">The type to be resolved.</typeparam>
        /// <returns>An instance for the specified type.</returns>
        public T Resolve<T>()
        {
            this.CheckNotValidTypes<T>();

            try
            {
                System.Console.WriteLine($"Resolving {typeof(T).Name}...");
                return this.iocContatiner.Resolve<T>();
            }
            catch (ContainerException ex)
            {
                System.Console.WriteLine("Resolve Container exception!");
                if (ex.Error == Error.ExpectedSingleDefaultFactory)
                {
                    throw new InvalidOperationException($"Expecting single default registration but found many for {typeof(T).Name}.");
                }

                System.Console.WriteLine(ex);

                System.Console.WriteLine($"Returning default: {default(T)}!");
                return default(T);
            }
        }

        /// <summary>
        /// Resolves instance of type from container.
        /// </summary>
        /// <param name="type">The type to be resolved.</param>
        /// <returns>An instance for the specified type.</returns>
        public object Resolve(Type type)
        {
            this.CheckNotValidTypes(type);
            return this.iocContatiner.Resolve(type, IfUnresolved.ReturnDefault);
        }

        /// <summary>
        /// Returns all registered instances of type <typeparamref name="T"/> from container.
        /// </summary>
        /// <typeparam name="T">The type to be resolved.</typeparam>
        /// <returns>An enumerable collection containing all registered instances.</returns>
        public IEnumerable<T> ResolveMany<T>()
        {
            this.CheckNotValidTypes<T>();
            return this.iocContatiner.ResolveMany<T>();
        }

        /// <summary>
        /// Returns all registered instances of type <typeparamref param="type"/> from container.
        /// </summary>
        /// <param name="type">The type to be resolved.</param>
        /// <returns>An enumerable collection containing all registered instances.</returns>
        public IEnumerable<object> ResolveMany(Type type)
        {
            this.CheckNotValidTypes(type);
            return this.iocContatiner.ResolveMany(type);
        }

        private void CheckNotValidTypes<TImplementation>()
        {
            this.CheckNotValidTypes(typeof(TImplementation));
        }

        private void CheckNotValidTypes(Type implementationType)
        {
            //if (typeof(Scene) == implementationType ||
            //                typeof(Entity) == implementationType ||
            //                typeof(Component) == implementationType)
            //{
            //    throw new InvalidOperationException($"{implementationType.Name} cannot be a {nameof(Scene)}, {nameof(Entity)} nor {nameof(Component)}");
            //}
        }

        private void CheckIsRegistered<TImplementation>()
        {
            this.CheckIsRegistered(typeof(TImplementation));
        }

        private void CheckIsRegistered(Type implementationType)
        {
            if (this.iocContatiner.IsRegistered(implementationType))
            {
                throw new InvalidOperationException($"There is a type already registered for {implementationType.Name}.");
            }
        }

        private IEnumerable<ContainerInterceptor> GetInterceptors<T>()
        {
            foreach (var interceptor in this.interceptors)
            {
                if (ReflectionHelper.IsAssignableFrom(interceptor.Type, typeof(T)))
                {
                    yield return interceptor;
                }
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.iocContatiner?.Dispose();
        }
    }
}
