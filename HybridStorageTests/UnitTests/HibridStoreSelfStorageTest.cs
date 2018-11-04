using HybridStorage;
using HybridStorageTests.TestModel.AdvancedTestModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace HybridStorageTests.UnitTests
{
    [SelfStored("SelfStorage")]
    public class SelfStoredContent
    {
        public string Id { get; set; }
        public string Language { get; set; }
        public List<Field> Fields { get; set; }
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
            // ARRANGE
            content.SelfStorage = JsonConvert.SerializeObject(new
            {
                Id = testId,
                Language = testLanguage,
                Fields = new List<Field> { new Field { Name = "FieldName", Value = "FieldValue" } }
            });

            // ACT
            modelStore.LoadStoredModels(content);

            // ASSERT
            Assert.AreEqual(testId, content.Id);
            Assert.AreEqual(testLanguage, content.Language);
            Assert.AreEqual("FieldName", content.Fields[0].Name);
            Assert.AreEqual("FieldValue", content.Fields[0].Value);

        }


        [TestMethod]
        [TestCategory(TestConstants.UnitTest)]
        public void StoreSelfStored()
        {
            // ARRANGE
            content = new SelfStoredContent
            {
                Id = testId,
                Language = testLanguage,
                Fields = new List<Field> { new Field { Name = "FieldName", Value = "FieldValue" } }
            };

            // ACT
            modelStore.StoreStoredModels(content);

            // ASSERT
            Assert.AreEqual(serializer.Serialize(content), content.SelfStorage);
        }
    }
}
