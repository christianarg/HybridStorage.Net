using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HybridStorageTests.TestModel.Inheritance;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HybridStorage;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Newtonsoft.Json;


namespace HybridStorageTests.IntegrationTests
{
    // TODO: "fusionar" con AsContainerTest
    [TestClass]
    public class HybridNewInheritanceTest : IntegrationTestBase
    {

        private const string fieldsname = "fieldsName";
        private const string fieldvalue = "fieldValue";
        private const string contentid = "contentId";

        [TestMethod]
        [TestCategory(TestConstants.IntegrationTest)]
        public void StoreAndRetrieveObjectTest()
        {
            // ARRANGE 
            CreateContent();

            using (var ctx = new IntegrationTestDbContext())
            {
                // ACT
                var infoContent = ctx.ContentContainer.HybridFind<ContentContainer,InfoContent>(contentid);
                // ASSERT
                Assert.IsNotNull(infoContent);
                var field = infoContent.Fields[0];
                Assert.AreEqual(fieldsname, field.Name);
                Assert.AreEqual(fieldvalue, field.Value);
            }
        }

        private static void CreateContent()
        {
            using (var ctx = new IntegrationTestDbContext())
            {
                var fields = new List<Field> { new Field() { Name = fieldsname, Value = fieldvalue } };
                var content = new InfoContent(contentid) { Fields = fields };

                ctx.ContentContainer.HybridAdd(content);
                ctx.SaveChanges();
            }
        }
        

        // Para comparar con PerformaceTPH.. los resultados son muy parecidos
        // por lo que probablmente el contenedor Hybrid no es necesario
        [TestMethod]
        [TestCategory(TestConstants.Performance)]
        public void QueryContainerPerformace()
        {
            using (var ctx = new IntegrationTestDbContext())
            {
                var fields = new List<Field> { new Field() { Name = fieldsname, Value = fieldvalue } };
                var content = new InfoContent(contentid) { Fields = fields };

                ctx.ContentContainer.HybridAdd(content);
                ctx.SaveChanges();
            }

            using (var ctx = new IntegrationTestDbContext())
            {
                var watch = new Stopwatch();
                watch.Start();
                for (int i = 0; i < 5000; i++)
                {
                    var infoContent = ctx.ContentContainer.SingleOrDefault(c => c.Id == contentid);
                }
                watch.Stop();
                Trace.Write(watch.Elapsed);
            }
        }
    }

    // Modelo que utiliza herencia TPH "normal" de EF
    // para mi grata sorpresa, 
    
    [SelfStored("Data")]
    public abstract class BaseSelfStoredContent
    {
        public string Language { get; set; }
        public string Id { get; set; }

        // De momento es necesario
        [JsonIgnore]
        public string Data { get; set; }
    }

    
    public class TheInfoContent : BaseSelfStoredContent
    {
        [NotMapped]
        public string Properties { get; set; }
    }
    
    public class TheResourceContent : BaseSelfStoredContent
    {
        [NotMapped]
        public string TheResource { get; set; }
    }



    // TODO: Mover a solución performancetests
    [TestClass]
    public class PerformaceTPHTests : IntegrationTestBase
    {

        [TestMethod]
        [TestCategory(TestConstants.Performance)]
        public void TPHQueryTest()
        {
            // ARRANGE
            using (var ctx = new IntegrationTestDbContext())
            {
                var content = new TheInfoContent()
                {
                    Id = "theid"
                    ,Language = "es-es"
                    ,Properties = "properties"
                };
                ctx.BaseContents.Add(content);
                ctx.SaveChanges();
            }


            using (var ctx = new IntegrationTestDbContext())
            {
                ctx.Configuration.AutoDetectChangesEnabled = false;

                var watch = new Stopwatch();
                watch.Start();
                var query = ctx.BaseContents.OfType<TheInfoContent>();
                for (int i = 0; i < 5000; i++)
                {
                    var cnt = query.SingleOrDefault(c => c.Id == "theid"); 
                }
                watch.Stop();
                Trace.Write(watch.Elapsed);
            }

        }

        [TestMethod]
        public void When_NoSelfStoredModelModified_Then_ContainerEntityIsNotSaved()
        {
            // TODO: Self Stored Integration Tests

            // ARRANGE
            using (var ctx = new IntegrationTestDbContext())
            {
                var content = new TheInfoContent()
                {
                    Id = "theid"
                    ,
                    Language = "es-es"
                    ,
                    Properties = "properties"
                };
                ctx.BaseContents.Add(content);
                ctx.SaveChanges();
            }

            // ACT
            using (var ctx = new IntegrationTestDbContext())
            {
                // Leemos el contenido pero no modificamos ningun stored model
                // Hay que comprobar que en este caso, el sistema no lo detecta como modificación
                var content = ctx.BaseContents.Find("theid");

                ctx.efToHybridStore.EntriesProcessed += (object sender, EFToHybridStore.EntriesProcessedEvent args) =>
                {
                    // ASSERT
                    Assert.AreEqual(0, args.Entities.Count);
                };

                ctx.SaveChanges();
            }
        }

        [TestMethod]
        [TestCategory(TestConstants.Performance)]
        public void OfTypeTest()
        {
            using (var ctx = new IntegrationTestDbContext())
            {
                var watch = new Stopwatch();
                watch.Start();
                for (int i = 0; i < 500000; i++)
                {
                    var kito = ctx.BaseContents.OfType<TheInfoContent>();
                }
                watch.Stop();
                Trace.Write(watch.Elapsed);
            }
        }

        [TestMethod]
        [TestCategory(TestConstants.Performance)]
        public void WithoutOfTypeTest()
        {
            using (var ctx = new IntegrationTestDbContext())
            {
                var watch = new Stopwatch();
                watch.Start();
                for (int i = 0; i < 500000; i++)
                {
                    
                    var kito = ctx.ContentContainer;
                }
                watch.Stop();
                Trace.Write(watch.Elapsed);
            }
        }
    }
    
}
