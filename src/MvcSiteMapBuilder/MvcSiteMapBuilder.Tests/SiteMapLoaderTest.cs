using NUnit.Framework;
using FakeItEasy;
using MvcSiteMapBuilder.Cache;

namespace MvcSiteMapBuilder.Tests
{
    [TestFixture]
    public class SiteMapLoaderTest
    {
        #region Setup/Teardown

        private ISiteMapCache siteMapCache = null;
        private ISiteMapCacheKeyGenerator siteMapCacheKeyGenerator = null;
        private ISiteMapBuilder siteMapBuilder = null;
        private ISiteMapCacheKeyToBuilderSetMapper siteMapCacheKeyToBuilderSetMapper = null;
        private ISiteMapBuilderSetStrategy siteMapBuilderSetStrategy = null;

        [SetUp]
        public void Setup()
        {
            siteMapCache = A.Fake<ISiteMapCache>();
            siteMapCacheKeyGenerator = A.Fake<ISiteMapCacheKeyGenerator>();
            siteMapBuilder = A.Fake<ISiteMapBuilder>();
            siteMapCacheKeyToBuilderSetMapper = A.Fake<ISiteMapCacheKeyToBuilderSetMapper>();
            siteMapBuilderSetStrategy = A.Fake<ISiteMapBuilderSetStrategy>();
        }

        [TearDown]
        public void TearDown()
        {

        }

        private ISiteMapLoader NewSiteMapLoader()
        {
            return new SiteMapLoader(siteMapBuilder, siteMapCacheKeyGenerator, siteMapCacheKeyToBuilderSetMapper, siteMapBuilderSetStrategy);
        }

        #endregion

        [Test]
        public void Test()
        {
            
        }


    }
}
