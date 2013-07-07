using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HybridStorageTests.IntegrationTests
{
    [TestClass]
    public abstract class IntegrationTestBase
    {
        [TestInitialize]
        public void BaseInit()
        {
            using (var ctx = new IntegrationTestDbContext(disableHybridStore: true))
            {
                ctx.DisableHybridStore = true;
                ctx.Database.Delete();
                ctx.Database.Create();

                ctx.SaveChanges();
            }
        }
    }
}
