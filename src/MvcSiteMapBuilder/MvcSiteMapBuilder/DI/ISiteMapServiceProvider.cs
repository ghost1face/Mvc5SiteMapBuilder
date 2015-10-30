namespace MvcSiteMapBuilder.DI
{
    public interface ISiteMapServiceProvider
    {
        /// <summary>
        /// Get an instance of the requested services. Note all services are singletones; multiple calls to Resolve will all return the same instance.
        /// </summary>
        /// <typeparam name="TService">The type of service to return</typeparam>
        /// <returns></returns>
        TService Resolve<TService>() where TService : class;
    }
}
