using System;
using System.Collections.Generic;
using System.Text;

namespace WasmApp1
{
    using DryIoc;

    public class Container : IDisposable
    {
        private DryIoc.Container iocContainer;

        public Container()
        {
            this.iocContainer = new DryIoc.Container(this.ContainerRules);
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

        public void RegisterInstance<TImplementation>(TImplementation instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var implementedTypes = typeof(TImplementation).GetImplementedServiceTypes(true);
            foreach (var type in implementedTypes)
            {
                this.iocContainer.RegisterDelegate(type, (c) => instance);
            }

            //foreach (var interceptor in this.GetInterceptors<TImplementation>())
            //{
            //    interceptor.OnRegistered(typeof(TImplementation));
            //    interceptor.OnInstanced(instance);
            //}
        }

        public void Register(Type serviceType, Type implementationType, bool singleton = true)
        {
            this.iocContainer.Register(serviceType, implementationType, singleton ? null : Reuse.Transient);
        }

        public void RegisterType<TImplementation>(bool singleton = true)
        {
            this.iocContainer.RegisterMany<TImplementation>(singleton ? null : Reuse.Transient, made: FactoryMethod.ConstructorWithResolvableArgumentsIncludingNonPublic, nonPublicServiceTypes: true);

            //foreach (var interceptor in this.GetInterceptors<TImplementation>())
            //{
            //    System.Console.WriteLine($"Interceptor: {interceptor}");
            //    interceptor.OnRegistered(typeof(TImplementation));
            //    this.iocContainer.RegisterInitializer<TImplementation>((s, c) => interceptor.OnInstanced(s));
            //}
        }

        public void RegisterType<T, TImplementation>(bool singleton = true)
            where TImplementation : T
        {
            this.iocContainer.Register<T, TImplementation>(singleton ? null : Reuse.Transient);

            //foreach (var interceptor in this.GetInterceptors<TImplementation>())
            //{
            //    interceptor.OnRegistered(typeof(TImplementation));
            //    this.iocContainer.RegisterInitializer<TImplementation>((s, c) => interceptor.OnInstanced(s));
            //}
        }

        //public void RegisterInterceptor(ContainerInterceptor containerInterceptor)
        //{
        //    if (containerInterceptor.Container != null)
        //    {
        //        throw new InvalidOperationException("The interceptor is already registered to a container");
        //    }

        //    this.interceptors.Add(containerInterceptor);
        //}

        public void Unregister<T>()
        {
            if (this.iocContainer.IsRegistered<T>())
            {
                var implementedTypes = typeof(T).GetImplementedServiceTypes(true);
                foreach (var type in implementedTypes)
                {
                    this.iocContainer.Unregister(type, condition: f => f.ImplementationType == typeof(T));
                }

                //foreach (var interceptor in this.GetInterceptors<T>())
                //{
                //    interceptor.OnUnregistered(typeof(T));
                //}
            }
        }

        public void Unregister<T>(T instance)
        {
            this.Unregister<T>();
        }

        public bool IsRegistered<T>()
        {
            return this.iocContainer.IsRegistered<T>();
        }

        public T Resolve<T>()
        {
            try
            {
                System.Console.WriteLine($"Resolving {typeof(T).Name}...");
                return this.iocContainer.Resolve<T>();
            }
            catch (ContainerException ex)
            {
                if (ex.Error == Error.ExpectedSingleDefaultFactory)
                {
                    throw new InvalidOperationException($"Expecting single default registration but found many for {typeof(T).Name}.");
                }

                //return default(T);
                throw;
            }
        }

        public object Resolve(Type type)
        {
            return this.iocContainer.Resolve(type, IfUnresolved.ReturnDefault);
        }

        public IEnumerable<T> ResolveMany<T>()
        {
            return this.iocContainer.ResolveMany<T>();
        }

        public IEnumerable<object> ResolveMany(Type type)
        {
            return this.iocContainer.ResolveMany(type);
        }

        //private IEnumerable<ContainerInterceptor> GetInterceptors<T>()
        //{
        //    foreach (var interceptor in this.interceptors)
        //    {
        //        if (ReflectionHelper.IsAssignableFrom(interceptor.Type, typeof(T)))
        //        {
        //            yield return interceptor;
        //        }
        //    }
        //}

        public void Dispose()
        {
            this.iocContainer?.Dispose();
        }
    }
}
