using System;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using HybridStorage;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using HybridStorageTests.TestModel.Inheritance;

namespace HybridStorageTests.UnitTests
{
    [TestClass]
    public class HybridStoreInheritanceTest
    {
        // TODO: Test casos no esperados (null en el storage, etc). Al menos crear excepciones especializadas
        private Content content;
        private HybridStore modelStore;
        private NewtonSoftJsonHybridStoreSerializer serializer;

        [TestInitialize]
        public void Init()
        {
            serializer = new NewtonSoftJsonHybridStoreSerializer();
            // De hecho esto lo convierte en un test más de integración que unitario...
            modelStore = new HybridStore(serializer);
            
        }


        [TestMethod]
        [TestCategory(TestConstants.UnitTest)]
        public void LoadInheritanceStoredModelsTest()
        {
            // ARRANGE
            content = new InfoContent("contentId");
            content.Language = "es-es";
            var contentContainer = new ContentContainer();
            contentContainer.Data = serializer.Serialize(content);
            // ACT
            modelStore.LoadStoredModels(contentContainer);
            // ASSERT
            content = contentContainer.Model;

            Assert.AreEqual(content.Id, "contentId");
            Assert.AreEqual(content.Language, "es-es");
        }

        [TestMethod]
        [TestCategory(TestConstants.UnitTest)]
        public void StoreInheritanceStoredModelsTest()
        {
            // ARRANGE
            content = new InfoContent("contentId") {Language = "es"};
            var contentContainer = new ContentContainer(content);
            
            // ACT
            modelStore.StoreStoredModels(contentContainer);
            
            // ASSERT
            Assert.AreEqual("es", contentContainer.Language);
            Assert.AreEqual("contentId", contentContainer.Id);
            Assert.AreEqual(contentContainer.Data,serializer.Serialize(content));
        }


        [TestMethod]
        [TestCategory(TestConstants.UnitTest)]
        public void EntityContainerBaseWithIdTest()
        {
            var containedEntity = new TheContainedEntity() { Id = "entityId" };
            var container = new EntityContainer(containedEntity);

            Assert.AreEqual(containedEntity.Id,container.Id);
        }

        public class EntityContainer : HybridEntityContainerBase<TheContainedEntity>
        {
            public string Id { get; set; }

            public EntityContainer(TheContainedEntity theContainedEntity) 
                : base(theContainedEntity)
            {
            }
        }
        public class TheContainedEntity
        {
            public string Id { get; set; }
        }

        [TestMethod]
        [TestCategory(TestConstants.UnitTest)]
        public void ObjectAutoMapperTest()
        {
            // ARRANGE
            object content = new InfoContent("contentId") { Language = "es" };
            var contentContainer = new ContentContainer(content as InfoContent);
            Mapper.DynamicMap(content, contentContainer,typeof(InfoContent),typeof(ContentContainer));
            Assert.AreEqual("es", contentContainer.Language);
        }

        [TestMethod]
        [TestCategory(TestConstants.UnitTest)]
        public void SimpleAutoMapperTestDynamic()
        {
            // ARRANGE
            content = new InfoContent("contentId") { Language = "es" };
            var contentContainer = new ContentContainer(content);
            Mapper.DynamicMap(content, contentContainer);
            Assert.AreEqual("es", contentContainer.Language);
        }

        [TestMethod]
        [TestCategory(TestConstants.UnitTest)]
        public void SimpleAutoMapperTestForDummies()
        {
            Mapper.CreateMap<Content, ContentContainer>()
                .Include<InfoContent, ContentContainer>();
            // ARRANGE
            content = new InfoContent("contentId") {Language = "es"};
            var contentContainer = new ContentContainer(content);
            Mapper.Map(content, contentContainer);
            Assert.AreEqual("es",contentContainer.Language);
        }
    }
}
