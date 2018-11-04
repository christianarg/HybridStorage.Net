using System;
using HybridStorageTests.TestModel.Inheritance;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Data.Entity;
using HybridStorage;
using HybridStorageTests.TestModel;
using HybridStorageTests.TestModel.SimpleTestModel;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace HybridStorageTests.IntegrationTests
{
    [TestClass]
    public class HybridStoreAsContainerTest
    {
        private const string fieldsname = "fieldsName";
        private const string fieldvalue = "fieldValue";
        private const string contentid = "contentId";
        ContentRepository repository;
       
        [TestInitialize]
        public void Init()
        {
            
            using (var ctx = new IntegrationTestDbContext(disableHybridStore: true))
            {
                ctx.DisableHybridStore = true;
                ctx.Database.Delete();
                ctx.Database.Create();

                ctx.SaveChanges();
            }
        }
        [TestMethod]
        [TestCategory(TestConstants.IntegrationTest)]
        public void StoreAndRetrieveObjectTest()
        {
            Mapper.Initialize(cfg => {
                cfg.CreateMap<InfoContent, ContentContainer>();
            });

            using (var ctx = new IntegrationTestDbContext())
            {
                repository = new ContentRepository(ctx); // Normalmente utilizariamos DI!! (interfaz, contenedor, etc)
                var fields = new List<Field> {new Field() {Name = fieldsname, Value = fieldvalue}};
                var content = new InfoContent(contentid) { Fields = fields };
                repository.Add(content);
                
                ctx.SaveChanges();
            }

            using (var ctx = new IntegrationTestDbContext())
            {
                repository = new ContentRepository(ctx); // Normalmente utilizariamos DI!! (interfaz, contenedor, etc)

                var infoContent = repository.Find<InfoContent>(contentid);
                Assert.IsNotNull(infoContent);
                var field = infoContent.Fields[0];
                Assert.AreEqual(fieldsname, field.Name);
                Assert.AreEqual(fieldvalue, field.Value);
            }
        }


        [TestMethod]
        [TestCategory(TestConstants.Performance)]
        public void QueryTest()
        {
            // ARRANGE
            using (var ctx = new IntegrationTestDbContext())
            {
                repository = new ContentRepository(ctx); // Normalmente utilizariamos DI!! (interfaz, contenedor, etc)

                repository.Add(new InfoContent("contentes") { Language = "es" });
                repository.Add(new InfoContent("contentes2") { Language = "es" });
                repository.Add(new InfoContent("contentcat") { Language = "cat" });
                
                ctx.SaveChanges();
            }

             // ARRANGE
            using (var ctx = new IntegrationTestDbContext())
            {
                repository = new ContentRepository(ctx); // Normalmente utilizariamos DI!! (interfaz, contenedor, etc)

                var contents = repository.GetContentByLanguage<InfoContent>("es");

                Assert.IsTrue(contents.Any(c => c.Id == "contentes"));
                Assert.IsTrue(contents.Any(c => c.Id == "contentes2"));
                Assert.IsFalse(contents.Any(c => c.Id == "contentcat"));
            }

        }
    }

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
            return contentContainer.Model as T;
        }

        public List<T> GetContentByLanguage<T>(string language)
             where T : Content
        {
            var containers = context.ContentContainer.Where(cc => cc.Language == language).ToList();

            return containers.Select(cc => cc.Model).Cast<T>().ToList();
        }
    }
}
