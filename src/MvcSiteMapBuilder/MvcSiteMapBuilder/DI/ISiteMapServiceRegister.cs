using System;

namespace MvcSiteMapBuilder.DI
{
    public interface ISiteMapServiceRegister
    {
        /// <summary>
        /// Register a service with a factory method.  First registration wins. All subsequent registrations will be ignored.
        /// </summary>
        /// <typeparam name="TService">The type of the service to be registered</typeparam>
        /// <param name="serviceCreator">A function that can create an instance of the service</param>
        /// <returns></returns>
        ISiteMapServiceRegister Register<TService>(Func<ISiteMapServiceProvider, TService> serviceCreator) where TService : class;

        /// <summary>
        /// Register a service. First registration wins. All subsequent registrations will be ignored.
        /// </summary>
        /// <typeparam name="TService">The type of the service to be registered</typeparam>
        /// <typeparam name="TImplementation">The implementation type</typeparam>
        /// <returns></returns>
        ISiteMapServiceRegister Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;
    }
}
