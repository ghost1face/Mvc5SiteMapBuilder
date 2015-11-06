namespace Mvc5SiteMapBuilder.DI
{
    /// <summary>
    /// Combines <typeparamref name="ISiteMapServiceProvider"/> and <typeparamref name="ISiteMapServiceRegister"/> as an implementation of an IoC container.
    /// </summary>
    public interface ISiteMapContainer : ISiteMapServiceProvider, ISiteMapServiceRegister
    {

    }
}
