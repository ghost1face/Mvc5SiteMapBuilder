using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MvcSiteMapBuilder.DI;
using MvcSiteMapBuilder.DataSource;
using System.Web.Hosting;
using MvcSiteMapBuilder.Cache;

namespace MvcSiteMapBuilder
{
    public class SiteMapConfiguration
    {
        #region Static Members

        private static SiteMapConfiguration configuration;

        public static SiteMapConfiguration Instance
        {
            get { return configuration; }
        }

        public static SiteMapConfiguration Init()
        {
            return Init(
                TimeSpan.FromMinutes(5),
                true,
                true
            );
        }

        public static SiteMapConfiguration Init(TimeSpan cacheDuration, bool enableSiteMapFile, bool securityTrimmingEnabled)
        {
            if (configuration != null)
                throw new InvalidOperationException("Configuration has already been initialized");

            configuration = new SiteMapConfiguration(cacheDuration)
            {
                Container = new SiteMapContainer(),

                CacheDuration = cacheDuration,
                EnableSiteMapFile = enableSiteMapFile,
                SecurityTrimmingEnabled = securityTrimmingEnabled
            };

            return configuration;
        }

        #endregion

        private SiteMapConfiguration(TimeSpan cacheDuration)
        {
            var absoluteFileName = HostingEnvironment.MapPath("~/mvc.sitemap.xml");
            builderSets = new Collection<ISiteMapBuilderSet>
            {
                new SiteMapBuilderSet(
                    "default",
                    new FileXmlSource(absoluteFileName),
                    new CacheDetails(cacheDuration, TimeSpan.MinValue, new RuntimeFileCacheDependency(absoluteFileName))
                )
            };
        }

        public SiteMapConfiguration RegisterBuilderSet(ISiteMapBuilderSet builderSet)
        {
            builderSets.Add(builderSet);

            return this;
        }

        public SiteMapConfiguration Register(Action<ISiteMapContainer> registration)
        {
            if (containerIsBuilt)
                throw new InvalidOperationException("Unable to register components/services after Configuration has been built.");

            if (registration == null)
                throw new ArgumentNullException(nameof(registration));

            registration(Container);

            return this;
        }

        public void Build()
        {
            if (containerIsBuilt)
                return;

            // register builder sets
            Container.Register(provider => builderSets.ToArray());

            // register all defaults
            SiteMapDefaultRegistrations.RegisterServices(this);

            // assign to presentation layer singleton instance
            SiteMaps.Loader = Container.Resolve<ISiteMapLoader>();

            containerIsBuilt = true;
        }

        private bool containerIsBuilt;
        private readonly ICollection<ISiteMapBuilderSet> builderSets;

        public ISiteMapContainer Container { get; private set; }
        public TimeSpan CacheDuration { get; private set; }
        public bool EnableSiteMapFile { get; private set; }
        public bool SecurityTrimmingEnabled { get; private set; }
    }
}
