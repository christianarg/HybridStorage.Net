using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Data.Entity;
using HybridStorage;
using HybridStorageTests.TestModel;
using HybridStorageTests.TestModel.SimpleTestModel;
using HybridStorageTests.TestModel.Inheritance;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;

namespace HybridStorageTests.IntegrationTests
{

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
            // ACT
            CreateContent();
            // ASSERT
            using (var ctx = new IntegrationTestDbContext())
            {
                var localization = GetLocalization(ctx);
                var version = localization.Version;
                Assert.IsNotNull(version);
                Assert.AreEqual(1, version.Number);
            }
        }

        [TestMethod]
        [TestCategory(TestConstants.IntegrationTest)]
        public void EditTest()
        {
            // ARRANGE

            CreateContent();
            // ACT
            using (var ctx = new IntegrationTestDbContext())
            {
                var localization = GetLocalization(ctx);
                localization.Version = new SimpleContentVersion() { Number = 2 };
                ctx.SaveChanges();
            }

            // ASSERT
            using (var ctx = new IntegrationTestDbContext())
            {
                var localization = GetLocalization(ctx);
                var version = localization.Version;
                Assert.IsNotNull(version);
                Assert.AreEqual(2, version.Number);
            }
        }

        [TestMethod]
        [TestCategory(TestConstants.IntegrationTest)]
        public void RemoveTest()
        {
            // ARRANGE
            CreateContent();

            // ACT
            using (var ctx = new IntegrationTestDbContext())
            {
                var localization = GetLocalization(ctx);
                localization.Version = null;
                ctx.SaveChanges();
            }
            // Assert
            using (var ctx = new IntegrationTestDbContext())
            {
                var localization = GetLocalization(ctx);
                
                Assert.IsNull(localization.Version);
            }
        }

        [TestMethod]
        public void When_NoStoredModelModified_Then_ContainerEntityIsNotSaved()
        {
            // ARRANGE
            CreateContent();

            // ACT
            using (var ctx = new IntegrationTestDbContext())
            {
                // Leemos el contenido pero no modificamos ningun stored model
                // Hay que comprobar que en este caso, el sistema no lo detecta como modificación
                var content = ctx.Contents.Find(testlocalizationId);
                
                ctx.efToHybridStore.EntriesProcessed += (object sender, EFToHybridStore.EntriesProcessedEvent args) => 
                {
                    // ASSERT
                    Assert.AreEqual(0, args.Entities.Count);
                };
                
                ctx.SaveChanges();
            }
        }


        [TestMethod]
        public void When_NoSelfStoredModelModified_Then_ContainerEntityIsNotSaved()
        {
            Assert.Fail();
        }

        private static void CreateContent()
        {
            using (var ctx = new IntegrationTestDbContext())
            {
                var localization = new SimpleContent()
                {
                    Id = testlocalizationId
                    ,
                    Language = "es-ES"
                    ,
                    Version = new SimpleContentVersion() { Number = 1 }
                };

                ctx.Contents.Add(localization);
                ctx.SaveChanges();
            }
        }

        private static SimpleContent GetLocalization(IntegrationTestDbContext ctx)
        {
            var localization = ctx.Contents.Find(testlocalizationId);
            return localization;
        }
    }
}
