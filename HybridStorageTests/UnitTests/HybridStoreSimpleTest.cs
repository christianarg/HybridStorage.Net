using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using HybridStorageTests.TestModel;
using HybridStorage;
using HybridStorageTests.TestModel.SimpleTestModel;

namespace HybridStorageTests.UnitTests
{
    [TestClass]
    public class HybridStoreSimpleTest
    {
        private SimpleContent localization;
        private HybridStore modelStore;
        private NewtonSoftJsonHybridStoreSerializer serializer;

        [TestInitialize]
        public void Init()
        {
            serializer = new NewtonSoftJsonHybridStoreSerializer();
            // De hecho esto lo convierte en un test más de integración que unitario...
            modelStore = new HybridStore(serializer);  
            localization = new SimpleContent();
        }

        [TestMethod]
        [TestCategory(TestConstants.UnitTest)]
        public void LoadStoredModelsTest()
        {
            // ARRANGE
            // TODO: serializer?
            localization.VersionStorage = JsonConvert.SerializeObject(new SimpleContentVersion() { Number = 1 });
            // ACT
            modelStore.LoadStoredModels(localization);
            // ASSERT
            Assert.IsNotNull(localization.Version);
            Assert.AreEqual(1, localization.Version.Number);
        }

        [TestMethod]
        [TestCategory(TestConstants.UnitTest)]
        public void StoreStoredModelsTest()
        {
            // ARRANGE
            var version = new SimpleContentVersion() { Number = 1 };
            localization.Version = version;
            // ACT
            modelStore.StoreStoredModels(localization);
            // ASSERT
            Assert.AreEqual(serializer.Serialize(version), localization.VersionStorage);
        }
    }
}
