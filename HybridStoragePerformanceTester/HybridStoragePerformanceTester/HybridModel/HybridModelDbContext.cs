using HybridPerformanceTests.Hybrid;
using HybridStorage;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HybridStoragePerformanceTester.HybridModel
{
    public class HybridDbContextDbContext : HybridStoreDbContext
    {
        public DbSet<HybridContent> HybridContents { get; set; }
    }
}
