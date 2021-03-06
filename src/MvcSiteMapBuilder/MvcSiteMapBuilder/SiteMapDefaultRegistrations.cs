﻿using System;
using Mvc5SiteMapBuilder.Cache;
using Mvc5SiteMapBuilder.Providers;
using Mvc5SiteMapBuilder.Security;

namespace Mvc5SiteMapBuilder
{
    public static class SiteMapDefaultRegistrations
    {
        public static void RegisterServices(SiteMapConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var container = configuration.Container;

            // default service registration
            container
                    .Register(_ => container)
                    .Register<ISiteMapBuilder, SiteMapBuilder>()
                    .Register<ISiteMapCacheKeyGenerator, SiteMapCacheKeyGenerator>()
                    .Register<ISiteMapCacheKeyToBuilderSetMapper, SiteMapCacheKeyToBuilderSetMapper>()
                    .Register<ISiteMapBuilderSetStrategy, SiteMapBuilderSetStrategy>()
                    .Register<ISiteMapLoader, SiteMapLoader>()
                    .Register<IXmlSiteMapNodeProvider, XmlSiteMapNodeProvider>()
                    .Register<IJSONSiteMapNodeProvider, JSONSiteMapNodeProvider>()
                    .Register<IAclModule, AuthorizeAttributeAclModule>()
                    .Register<ICacheProvider<SiteMap>, InMemoryCacheProvider<SiteMap>>()
                    .Register<ISiteMapCache, SiteMapCache>();
        }
    }
}
