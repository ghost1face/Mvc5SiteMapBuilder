using System;
using System.Linq;
using MvcSiteMapBuilder.DataSource;
using MvcSiteMapBuilder.Extensions;
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

            var siteMapNodes = siteMapNodeProvider.GetSiteMapNodes(builderSet.DataSource).ToList();

            // resolve other information regarding the sitemap nodes
            siteMapNodes.ForEach(node => ResolveUrl(node));

            return new SiteMap
            {
                CacheKey = cacheKey ?? builderSet.BuilderSetName,
                Nodes = siteMapNodes
            };
        }

        #region Private Methods

        private void ResolveUrl(SiteMapNode rootNode)
        {
            rootNode.ResolveUrl();

            if(rootNode.HasChildNodes)
            {
                foreach(var node in rootNode.ChildNodes)
                {
                    ResolveUrl(node);
                }
            }
        }

        #endregion
    }
}
