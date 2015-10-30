using System;
using System.Linq;
using MvcSiteMapBuilder.DataSource;
using MvcSiteMapBuilder.Providers;

namespace MvcSiteMapBuilder
{
    public class SiteMapBuilder : ISiteMapBuilder
    {
        private readonly IXmlSiteMapNodeProvider xmlSiteMapNodeProvider;

        public SiteMapBuilder(IXmlSiteMapNodeProvider xmlSiteMapNodeProvider)
        {
            this.xmlSiteMapNodeProvider = xmlSiteMapNodeProvider;
        }

        public SiteMap BuildSiteMap(ISiteMapBuilderSet builderSet, string cacheKey)
        {
            if (builderSet == null)
                throw new ArgumentNullException(nameof(builderSet));

            ISiteMapNodeProvider siteMapNodeProvider = null;
            var xmlDataSource = builderSet.DataSource as ISiteMapXmlDataSource;
            if (xmlDataSource != null)
            {
                siteMapNodeProvider = xmlSiteMapNodeProvider;

                if (siteMapNodeProvider == null)
                    throw new Exception($"Provider of type {NodeProviderType.Xml} is not registered.");
            }

            // TODO: JSON support

            if (siteMapNodeProvider == null)
                throw new Exception("No supported provider found.");

            return new SiteMap
            {
                CacheKey = cacheKey ?? builderSet.BuilderSetName,
                Nodes = siteMapNodeProvider.GetSiteMapNodes(builderSet.DataSource).ToList()
            };
        }
    }
}
