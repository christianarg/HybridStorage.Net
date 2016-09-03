using HybridPerformanceTests.Hybrid;
using HybridPerformanceTests.NormalModel;
using HybridStorage;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HybridStoragePerformanceTester
{
    public class NormalDbContextDbContext : HybridStoreDbContext
    {
        public DbSet<NormalContent> NormalContents { get; set; }
    }
}
