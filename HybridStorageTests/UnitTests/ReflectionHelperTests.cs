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


        //[TestMethod]
        //[TestCategory(TestConstants.UnitTest)]
        //public void ReadSelfStoredAttribute()
        //{
        //    var attr = ReflectionHelper.GetStorageAttribute(
        //}
    }
}
