using System.Reflection;
using HybridStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HybridStorageTests.UnitTests
{
    [TestClass]
    public class ReflectionHelperTests
    {
        [TestMethod]
        [TestCategory(TestConstants.UnitTest)]
        public void ReadSelfStoredAttribute()
        {
            var attr = ReflectionHelper.ReadSelfStoredAttribute(new SelfStoredContent().GetType());
            Assert.IsNotNull(attr);
            Assert.AreEqual("SelfStorage", attr.StorageProperty);
        }


        [TestMethod]
        [TestCategory(TestConstants.UnitTest)]
        public void ReadStoredModelAndInheritanceContained()
        {
            var foo = new Foo() { Bar = new Bar() };

            PropertyInfo kito = ReflectionHelper.GetStoredModelProperties(foo.GetType()).FirstOrDefault();
            Assert.IsTrue(ReflectionHelper.HasAttribute<InheritanceContained>(kito));
            //var kito2 = ReflectionHelper.GetPropertyInfo<InheritanceContained>(foo.GetType()).FirstOrDefault();

            //Assert.AreEqual(kito,kito2);

        }

        public class Foo
        {
            public string BarData { get; set; }
            [StoredModel("BarData")]
            [InheritanceContained]
            public Bar Bar { get; set; }
        }
        public class Bar
        {
            
        }
        //[TestMethod]
        //[TestCategory(TestConstants.UnitTest)]
        //public void ReadSelfStoredAttribute()
        //{
        //    var attr = ReflectionHelper.GetStorageAttribute(
        //}
    }
}
