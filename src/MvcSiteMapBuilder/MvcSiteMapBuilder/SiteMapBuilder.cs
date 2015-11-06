using System;
using System.Linq;
using Mvc5SiteMapBuilder.DataSource;
using Mvc5SiteMapBuilder.Extensions;
using Mvc5SiteMapBuilder.Providers;

namespace Mvc5SiteMapBuilder
{
    public class SiteMapBuilder : ISiteMapBuilder
    {
        private readonly IXmlSiteMapNodeProvider xmlSiteMapNodeProvider;
        private readonly IJSONSiteMapNodeProvider jsonSiteMapNodeProvider;

        public SiteMapBuilder(IXmlSiteMapNodeProvider xmlSiteMapNodeProvider, IJSONSiteMapNodeProvider jsonSiteMapNodeProvider)
        {
            this.xmlSiteMapNodeProvider = xmlSiteMapNodeProvider;
            this.jsonSiteMapNodeProvider = jsonSiteMapNodeProvider;
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

            var jsonDataSource = builderSet.DataSource as ISiteMapJSONDataSource;
            if (jsonDataSource != null)
            {
                siteMapNodeProvider = jsonSiteMapNodeProvider;

                if (siteMapNodeProvider == null)
                    throw new Exception($"Provider of type {NodeProviderType.Json} is not registered.");
            }

            if (siteMapNodeProvider == null)
                throw new Exception("No supported provider found.");

            var siteMapNodes = siteMapNodeProvider.GetSiteMapNodes(builderSet.DataSource).ToList();

            // resolve other information regarding the sitemap nodes
            siteMapNodes.ForEach(ResolveUrl);

            return new SiteMap
            {
                CacheKey = cacheKey ?? builderSet.BuilderSetName,
                Nodes = siteMapNodes
            };
        }

        #region Private Methods

        private void ResolveUrl(SiteMapNode rootNode)
        {
            rootNode.Url = rootNode.ResolveUrl();

            if (rootNode.HasChildNodes)
            {
                foreach (var node in rootNode.ChildNodes)
                {
                    ResolveUrl(node);
                }
            }
        }

        #endregion
    }
}
