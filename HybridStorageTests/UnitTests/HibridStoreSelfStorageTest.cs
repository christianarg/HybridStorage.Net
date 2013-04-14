using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using HybridStorage;

namespace HybridStorageTests.UnitTests
{
    [SelfStored("SelfStorage")]
    public class SelfStoredContent
    {
        public string Id { get; set; }
        public string Language { get; set; }

        [JsonIgnore] // De momento es necesario
        public string SelfStorage { get; set; }
    }

    [TestClass]
    public class HibridStoreSelfStorageTest : HybridStoreTestBase
    {
        SelfStoredContent content;
 
        const string testId = "theId";
        const string testLanguage = "es-ES";

        [TestInitialize]
        public void Init()
        {
            content = new SelfStoredContent();
        }

        [TestMethod]
        [TestCategory(TestConstants.UnitTest)]
        public void LoadSelfStored()
        {
            content.SelfStorage = JsonConvert.SerializeObject(new { Id = testId, Language = testLanguage });
            modelStore.LoadStoredModels(content);

            Assert.AreEqual(testId, content.Id);
            Assert.AreEqual(testLanguage, content.Language);
        }


        [TestMethod]
        [TestCategory(TestConstants.UnitTest)]
        public void StoreSelfStored()
        {
            content = new SelfStoredContent() { Id = testId, Language = testLanguage };
            modelStore.StoreStoredModels(content);

            Assert.AreEqual(serializer.Serialize(content), content.SelfStorage);
        }
    }
}
