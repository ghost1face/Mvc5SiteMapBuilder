using System;
using System.Linq;
using Mvc5SiteMapBuilder.Cache;

namespace Mvc5SiteMapBuilder
{
    public class SiteMapBuilderSetStrategy : ISiteMapBuilderSetStrategy
    {
        protected readonly ISiteMapBuilderSet[] siteMapBuilderSets;

        public SiteMapBuilderSetStrategy(ISiteMapBuilderSet[] siteMapBuilderSets)
        {
            if (siteMapBuilderSets == null)
                throw new ArgumentNullException(nameof(siteMapBuilderSets));

            this.siteMapBuilderSets = siteMapBuilderSets;
        }

        public ISiteMapBuilderSet GetBuilderSet(string builderSetName)
        {
            var builderSet = siteMapBuilderSets.FirstOrDefault(x => x.AppliesTo(builderSetName));
            if(builderSet == null)
            {
                throw new Exception($"BuilderSet {builderSetName} not found.");
            }
            return builderSet;
        }

        public ICacheDetails GetCacheDetails(string builderSetName)
        {
            var builderSet = this.GetBuilderSet(builderSetName);
            return builderSet.CacheDetails;
        }
    }
}
