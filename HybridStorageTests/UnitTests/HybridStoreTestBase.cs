using HybridStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HybridStorageTests.UnitTests
{
    [TestClass]
    public class HybridStoreTestBase
    {
        protected HybridStore modelStore;
        protected NewtonSoftJsonHybridStoreSerializer serializer;

        [TestInitialize]
        public void BaseInit()
        {
            // TODO: Clase base test
            serializer = new NewtonSoftJsonHybridStoreSerializer();
            // De hecho esto lo convierte en un test más de integración que unitario...
            modelStore = new HybridStore(serializer);
        }
    }
}
