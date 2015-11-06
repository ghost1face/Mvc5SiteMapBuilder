using System;
using Mvc5SiteMapBuilder.Cache;
using Mvc5SiteMapBuilder.DataSource;

namespace Mvc5SiteMapBuilder
{
    public class SiteMapBuilderSet : ISiteMapBuilderSet
    {
        private readonly ICacheDetails cacheDetails;
        private readonly ISiteMapDataSource siteMapDataSource;
        private readonly string builderSetName;

        public SiteMapBuilderSet(string builderSetName, ISiteMapDataSource siteMapDataSource, ICacheDetails cacheDetails)
        {
            this.siteMapDataSource = siteMapDataSource;
            this.builderSetName = builderSetName;
            this.cacheDetails = cacheDetails;
        }

        public ICacheDetails CacheDetails
        {
            get { return cacheDetails; }
        }

        public string BuilderSetName
        {
            get { return builderSetName; }
        }

        public ISiteMapDataSource DataSource
        {
            get { return siteMapDataSource; }
        }

        public bool AppliesTo(string builderSetName)
        {
            return this.builderSetName.Equals(builderSetName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
