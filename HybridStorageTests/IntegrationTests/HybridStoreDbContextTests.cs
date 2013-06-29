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

namespace HybridStorageTests.IntegrationTests
{
    public class ContentRepository
    {
        private readonly IntegrationTestDbContext context;

        public ContentRepository(IntegrationTestDbContext context)
        {
            this.context = context;
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }

        public void Add(Content content)
        {
            var container = new ContentContainer(content);
            container.Language = content.Language;   // TODO: AutoMapper ? StoredModelDenormalized? Auto-AutoMapper :)
            context.ContentContainer.Add(container);
        }

        public void Remove(Content item)
        {
            var contentContainer = context.ContentContainer.Find(item.Id);
            if (contentContainer == null)
                return; // O excepción no se
            context.ContentContainer.Remove(contentContainer);
        }

        public T Find<T>(string id) where T : Content
        {
            var contentContainer = context.ContentContainer.Find(id);
            if (contentContainer == null)
                return null;
            return contentContainer.Content as T;
        }

        public List<T> GetContentByLanguage<T>(string language)
             where T : Content
        {
            var containers = context.ContentContainer.Where(cc => cc.Language == language).ToList();
            
            return containers.Select(cc => cc.Content).Cast<T>().ToList();
        }


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
