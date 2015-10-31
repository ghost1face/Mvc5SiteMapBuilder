using NUnit.Framework;
using FakeItEasy;
using MvcSiteMapBuilder.Cache;
using System;

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
            return new SiteMapLoader(siteMapBuilder, siteMapCacheKeyGenerator, siteMapCacheKeyToBuilderSetMapper, siteMapBuilderSetStrategy, siteMapCache);
        }

        #endregion

        [Test]
        public void GetSiteMap_NoParameterOverload_ShouldCallGenerateKeyAndPassResultToGetOrAdd()
        {
            // arrange
            var siteMapLoader = NewSiteMapLoader();
            A.CallTo(() => siteMapCacheKeyGenerator.GenerateKey()).Returns("theKey");

            // act
            var result = siteMapLoader.GetSiteMap();

            // assert 
            A.CallTo(() => siteMapCacheKeyGenerator.GenerateKey()).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => siteMapCache.GetOrAdd("theKey", A<Func<SiteMap>>.Ignored, A<ICacheDetails>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void GetSiteMap_EmptySiteMapCacheKey_ShouldCallGenerateKeyAndPassResultToGetOrAdd()
        {
            // arrange
            var siteMapCacheKey = string.Empty;
            var siteMapLoader = NewSiteMapLoader();

            A.CallTo(() => siteMapCacheKeyGenerator.GenerateKey()).Returns("theKey");

            // act
            var result = siteMapLoader.GetSiteMap(siteMapCacheKey);

            // assert
            A.CallTo(() => siteMapCacheKeyGenerator.GenerateKey()).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => siteMapCache.GetOrAdd("theKey", A<Func<SiteMap>>.Ignored, A<ICacheDetails>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void GetSiteMap_NullSiteMapCacheKey_ShouldCallGenerateKeyAndPassResultToGetOrAdd()
        {
            // arrange
            string siteMapCacheKey = null;
            var siteMapLoader = NewSiteMapLoader();

            A.CallTo(() => siteMapCacheKeyGenerator.GenerateKey()).Returns("theKey");

            // act
            var result = siteMapLoader.GetSiteMap(siteMapCacheKey);

            // assert
            A.CallTo(() => siteMapCacheKeyGenerator.GenerateKey()).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => siteMapCache.GetOrAdd("theKey", A<Func<SiteMap>>.Ignored, A<ICacheDetails>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void GetSiteMap_WithSiteMapCacheKey_ShouldCallGetOrAddWithKey()
        {
            // arrange
            var siteMapCacheKey = "myKey";
            var siteMapLoader = NewSiteMapLoader();

            // act
            var result = siteMapLoader.GetSiteMap(siteMapCacheKey);

            // assert
            A.CallTo(() => siteMapCacheKeyGenerator.GenerateKey()).MustNotHaveHappened();
            A.CallTo(() => siteMapCache.GetOrAdd(siteMapCacheKey, A<Func<SiteMap>>.Ignored, A<ICacheDetails>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void ReleaseSiteMap_NoParameterOverload_ShouldCallGenerateKeyAndPassResultToRemove()
        {
            // arrange
            var siteMapLoader = NewSiteMapLoader();
            A.CallTo(() => siteMapCacheKeyGenerator.GenerateKey()).Returns("theKey");

            // act
            siteMapLoader.ReleaseSiteMap();

            // assert
            A.CallTo(() => siteMapCacheKeyGenerator.GenerateKey()).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => siteMapCache.Remove("theKey")).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void ReleaseSiteMap_WithNullCacheKey_ShouldCallGenerateKeyAndPassResultToRemove()
        {
            // arrange
            string siteMapCacheKey = null;
            var siteMapLoader = NewSiteMapLoader();
            A.CallTo(() => siteMapCacheKeyGenerator.GenerateKey()).Returns("theKey");

            // act
            siteMapLoader.ReleaseSiteMap(siteMapCacheKey);

            // assert
            A.CallTo(() => siteMapCacheKeyGenerator.GenerateKey()).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => siteMapCache.Remove("theKey")).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void ReleaseSiteMap_EmptyCacheKey_ShouldCallGenerateKeyAndPassResultToRemove()
        {
            // arrange
            string siteMapCacheKey = string.Empty;
            var siteMapLoader = NewSiteMapLoader();
            A.CallTo(() => siteMapCacheKeyGenerator.GenerateKey()).Returns("theKey");

            // act
            siteMapLoader.ReleaseSiteMap(siteMapCacheKey);

            // assert
            A.CallTo(() => siteMapCacheKeyGenerator.GenerateKey()).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => siteMapCache.Remove("theKey")).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void ReleaseSiteMap_WithCacheKey_ShouldPassResultToRemove()
        {
            // arrange
            string siteMapCacheKey = "myKey";
            var siteMapLoader = NewSiteMapLoader();

            // act
            siteMapLoader.ReleaseSiteMap(siteMapCacheKey);

            // assert
            A.CallTo(() => siteMapCacheKeyGenerator.GenerateKey()).MustNotHaveHappened();
            A.CallTo(() => siteMapCache.Remove("myKey")).MustHaveHappened(Repeated.Exactly.Once);
        }

    }
}
