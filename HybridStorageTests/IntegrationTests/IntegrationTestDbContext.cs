using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using HybridStorage;
using HybridStorageTests.TestModel.SimpleTestModel;
using HybridStorageTests.TestModel.Inheritance;

namespace HybridStorageTests.IntegrationTests
{
    public class IntegrationTestDbContext : HybridStoreDbContext
    {
        public IntegrationTestDbContext(bool disableHybridStore = false )
            : base(disableHybridStore) { }
        public DbSet<SimpleContent> Contents { get; set; }

        public DbSet<ContentContainer> ContentContainer { get; set; }
        public DbSet<BaseSelfStoredContent> BaseContents { get; set; }

        public DbSet<Master> MasterModel { get; set; }
    }
}
