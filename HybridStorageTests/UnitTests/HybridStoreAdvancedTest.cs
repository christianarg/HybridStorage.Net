using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using HybridStorage;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using HybridStorageTests.TestModel.AdvancedTestModel;

namespace HybridStorageTests.UnitTests
{
    [TestClass]
    public class HybridStoreAdvancedTest
    {
        private Content localization;
        private HybridStore modelStore;
        private NewtonSoftJsonHybridStoreSerializer serializer;

        [TestInitialize]
        public void Init()
        {
            serializer = new NewtonSoftJsonHybridStoreSerializer();
            // De hecho esto lo convierte en un test más de integración que unitario...
            modelStore = new HybridStore(serializer);  
            localization = new Content();
        }


        [TestMethod]
        [TestCategory(TestConstants.UnitTest)]
        public void LoadStoredModelsTest()
        {
            // ARRANGE
            localization.VersionStorage = serializer.Serialize(CreateVersionModel());
            // ACT
            modelStore.LoadStoredModels(localization);
            // ASSERT
            var version = localization.Version;
            Assert.IsNotNull(version);
            Assert.AreEqual(1, version.Number);
            Assert.AreEqual(VersionType.Pwc, version.VersionType);
            Assert.AreEqual(1, version.Fields.Count);

            var field = version.Fields.First();
            Assert.AreEqual("Field1", field.Name);
            Assert.AreEqual("Value1", field.Value);
        }

        [TestMethod]
        [TestCategory(TestConstants.UnitTest)]
        public void StoreStoredModelsTest()
        {
            // ARRANGE
            var version = CreateVersionModel();
            localization.Version = version;
            // ACT
            modelStore.StoreStoredModels(localization);
            // ASSERT
            Assert.AreEqual(serializer.Serialize(version), localization.VersionStorage);
        }

        private ContentVersion CreateVersionModel()
        {
            var version = new ContentVersion() { Number = 1, VersionType = VersionType.Pwc };
            // TODO: IField Field y OtroField
            version.Fields.Add(new Field() { Name = "Field1", Value = "Value1" });
            return version;
        }
    }
}
