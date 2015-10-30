using System;
using System.Collections.Concurrent;
using System.Linq;

namespace MvcSiteMapBuilder.DI
{
    public class SiteMapContainer : ISiteMapContainer
    {
        private readonly ConcurrentDictionary<Type, object> factories = new ConcurrentDictionary<Type, object>();
        private readonly ConcurrentDictionary<Type, Type> registrations = new ConcurrentDictionary<Type, Type>();

        private readonly ConcurrentDictionary<Type, object> resolvedInstances = new ConcurrentDictionary<Type, object>();
        private readonly object syncLock = new object();

        public virtual TService Resolve<TService>()
            where TService : class
        {
            var typeToResolve = typeof(TService);
            object instance;

            if (resolvedInstances.TryGetValue(typeToResolve, out instance))
                return (TService)instance;
            lock (syncLock)
            {
                if (resolvedInstances.TryGetValue(typeToResolve, out instance))
                    return (TService)instance;

                Type registration;
                object factory;
                if (registrations.TryGetValue(typeToResolve, out registration))
                {
                    instance = CreateServiceInstance(registration);
                    resolvedInstances.TryAdd(typeToResolve, instance);
                }
                else if (factories.TryGetValue(typeToResolve, out factory))
                {
                    instance = ((Func<ISiteMapServiceProvider, TService>)factory)(this);
                    resolvedInstances.TryAdd(typeToResolve, instance);
                }
                else
                {
                    throw new InvalidOperationException($"No service of type {typeToResolve.Name} has been found.");
                }

                return (TService)instance;
            }
        }

        public virtual ISiteMapServiceRegister Register<TService>(Func<ISiteMapServiceProvider, TService> serviceCreator)
            where TService : class
        {
            if (serviceCreator == null)
                throw new ArgumentNullException(nameof(serviceCreator));

            lock (syncLock)
            {
                var serviceType = typeof(TService);
                if (IsServiceRegistered(serviceType))
                    return this;
                factories.TryAdd(serviceType, serviceCreator);
                return this;
            }
        }

        public virtual ISiteMapServiceRegister Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            var serviceType = typeof(TService);
            var implementationType = typeof(TImplementation);

            lock (syncLock)
            {
                if (IsServiceRegistered(serviceType))
                    return this;

                if (!serviceType.IsAssignableFrom(implementationType))
                {
                    throw new InvalidOperationException($"Registration type: {implementationType.Name} does not implement service interface {serviceType.Name}.");
                }

                var constructors = implementationType.GetConstructors();
                if (constructors.Length > 1)
                {
                    throw new InvalidOperationException($"Registered types must have only one constructor, registration type: {implementationType.Name} has {constructors.Length}.");
                }

                registrations.TryAdd(serviceType, implementationType);

                return this;
            }
        }

        #region Private Methods

        private bool IsServiceRegistered(Type serviceType)
        {
            return factories.ContainsKey(serviceType) || registrations.ContainsKey(serviceType);
        }

        private object CreateServiceInstance(Type serviceType)
        {
            var constructorInfo = serviceType.GetConstructors();

            var parameters = constructorInfo[0]
                .GetParameters()
                .Select(parameterInfo => Resolve(parameterInfo.ParameterType))
                .ToArray();

            return constructorInfo[0].Invoke(parameters);
        }

        private object Resolve(Type serviceType)
        {
            return typeof(SiteMapContainer)
                .GetMethod("Resolve", new Type[0])
                .MakeGenericMethod(serviceType)
                .Invoke(this, new object[0]);
        }

        #endregion
    }
}
