using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Hosting;
using Mvc5SiteMapBuilder.Cache;
using Mvc5SiteMapBuilder.DataSource;
using Mvc5SiteMapBuilder.DI;

namespace Mvc5SiteMapBuilder
{
    public class SiteMapConfiguration
    {
        #region Static Members

        private static SiteMapConfiguration configuration;

        /// <summary>
        /// Access an initialized instance of the SiteMapConfiguration
        /// </summary>
        public static SiteMapConfiguration Instance
        {
            get { return configuration; }
        }

        /// <summary>
        /// Initialize an instance of sitemap configuration
        /// </summary>
        /// <returns></returns>
        public static SiteMapConfiguration Init()
        {
            return Init
            (
                TimeSpan.FromMinutes(5),
                true,
                true
            );
        }

        public static SiteMapConfiguration Init(TimeSpan cacheDuration, bool enableSiteMapFile, bool securityTrimmingEnabled)
        {
            if (configuration != null)
                throw new InvalidOperationException("Configuration has already been initialized");

            configuration = new SiteMapConfiguration()
            {
                Container = new SiteMapContainer(),

                CacheDuration = cacheDuration,
                EnableSiteMapFile = enableSiteMapFile,
                SecurityTrimmingEnabled = securityTrimmingEnabled
            };

            return configuration;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates an instance of SiteMapConfiguration
        /// </summary>
        /// <param name="cacheDuration"></param>
        private SiteMapConfiguration()
        {
            builderSets = new Collection<ISiteMapBuilderSet>();
        }

        #endregion

        /// <summary>
        /// Registers a builderset with the current configuration (This is for supporting multiple sitemap files)
        /// </summary>
        /// <param name="builderSet"></param>
        /// <returns></returns>
        public SiteMapConfiguration RegisterBuilderSet(ISiteMapBuilderSet builderSet)
        {
            builderSets.Add(builderSet);

            return this;
        }

        /// <summary>
        /// Register an override of default implementation
        /// </summary>
        /// <param name="registration"></param>
        /// <returns></returns>
        public SiteMapConfiguration Register(Action<ISiteMapContainer> registration)
        {
            if (containerIsBuilt)
                throw new InvalidOperationException("Unable to register components/services after Configuration has been built.");

            if (registration == null)
                throw new ArgumentNullException(nameof(registration));

            registration(Container);

            return this;
        }

        /// <summary>
        /// Build IoC container with the specified configuration
        /// </summary>
        public void Build()
        {
            if (containerIsBuilt)
                return;

            if (!builderSets.Any())
            {
                var absoluteFileName = HostingEnvironment.MapPath("~/mvc.sitemap.xml");

                builderSets.Add(
                    new SiteMapBuilderSet(
                        "default",
                        new FileXmlSource(absoluteFileName),
                        new CacheDetails(CacheDuration, TimeSpan.MinValue, new RuntimeFileCacheDependency(absoluteFileName))
                    )
                );
            }

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
