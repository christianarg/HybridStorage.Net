using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Data.Entity;
using HybridStorage;
using HybridStorageTests.TestModel;
using HybridStorageTests.TestModel.SimpleTestModel;

namespace HybridStorageTests.IntegrationTests
{
    public class IntegrationTestDbContext : HybridStoreDbContext
    {
        public DbSet<SimpleContent> Contents { get; set; }
    }

    [TestClass]
    public class HybridStoreDbContextTests
    {
        const string testlocalizationId = "testContentId-es";
        
        [TestInitialize]
        public void Init()
        {
            using (var ctx = new IntegrationTestDbContext())
            {
                ctx.Database.Delete();
                ctx.Database.Create();

                ctx.SaveChanges();
            }
        }
        [TestMethod]
        [TestCategory(TestConstants.IntegrationTest)]
        public void StoreAndRetrieveObjectTest()
        {
            using (var ctx = new IntegrationTestDbContext())
            {
                var localization = new SimpleContent() 
                { 
                    Id = testlocalizationId
                    , Language = "es-ES"
                    , Version = new SimpleContentVersion() { Number = 1 }
                };

                ctx.Contents.Add(localization);
                ctx.SaveChanges();
            }
            using (var ctx = new IntegrationTestDbContext())
            {
                var localization = GetLocalization(ctx);
                var version = localization.Version;
                Assert.IsNotNull(version);
                Assert.AreEqual(1, version.Number);
            }
        }

        private static SimpleContent GetLocalization(IntegrationTestDbContext ctx)
        {
            var localization = ctx.Contents.Find(testlocalizationId);
            return localization;
        }
    }
}
