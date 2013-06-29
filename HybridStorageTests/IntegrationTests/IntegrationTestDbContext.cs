using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using HybridStorage;
using HybridStorageTests.TestModel.SimpleTestModel;

namespace HybridStorageTests.IntegrationTests
{
    public class IntegrationTestDbContext : HybridStoreDbContext
    {
        public IntegrationTestDbContext(bool disableHybridStore = false )
            : base(disableHybridStore) { }
        public DbSet<SimpleContent> Contents { get; set; }

        public DbSet<HybridStorageTests.TestModel.Inheritance.ContentContainer> ContentContainer { get; set; }

    }
}
