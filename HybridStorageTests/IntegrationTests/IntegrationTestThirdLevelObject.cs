using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HybridStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HybridStorageTests.IntegrationTests
{


    /// <summary>
    /// Tests a stored model being a "third level object".
    /// This means you have master => detail => StoredModel
    /// </summary>
    [TestClass]
    public class ThidLevelModelIntegrationTests
    {
        [TestInitialize]
        public void Init()
        {
            using (var ctx = new IntegrationTestDbContext(disableHybridStore: true))
            {
                ctx.Database.Delete();
                ctx.Database.Create();

                ctx.SaveChanges();
            }
        }
        [TestMethod]
        [TestCategory(TestConstants.IntegrationTest)]
        public void ThirdLevelStoredModel_StoreAndRetrieveObjectTest()
        {
            // ARRANGE
            using (var ctx = new IntegrationTestDbContext())
            {
                var arr = new Master() { Id = 1 };
                ctx.MasterModel.Add(arr);
                arr.Details = new List<Detail>() { new Detail() { Id = 1, ThirdLevelStoredModel = new ThirdLevelStoredModel() { Id = "novaunJoraca" } } };

                // ACT
                ctx.SaveChanges();
            }

            // ASSERT
            using (var ctx = new IntegrationTestDbContext())
            {
                var master = ctx.MasterModel.First();
                Assert.IsNotNull(master);
                var detail = master.Details.First();
                Assert.IsNotNull(detail);
                Assert.IsNotNull(detail.ThirdLevelStoredModel);

                Assert.AreEqual(detail.ThirdLevelStoredModel.Id, "novaunJoraca");
            }
        }


        //[TestMethod]
        //[TestCategory(TestConstants.IntegrationTest)]
        //public void TestPuto()
        //{
        //    // ARRANGE
        //    using (var ctx = new IntegrationTestDbContext())
        //    {
        //        var master = new Master() { Id = 1 };
        //        ctx.MasterModel.Add(master);

        //        master.Details = new List<Detail>()
        //        {
        //            new Detail()
        //            {
        //                Id = 1, 
        //                ThirdLevelStoredModel = new ThirdLevelStoredModel()
        //                {
        //                    Id = "novaunJoraca"
        //                }
        //            }
        //        };

        //        ctx.SaveChanges();

        //        var yeOldeObjectContext = ((IObjectContextAdapter)ctx).ObjectContext;
        //        Assert.IsTrue(ctx.ChangeTracker.Entries().Any(e => e.Entity == master), "Master not found in ChnageTracker");
        //        Assert.IsTrue(ctx.ChangeTracker.Entries().Any(e => e.Entity == master.Details.First()), "Detail not found in ChnageTracker");

        //        // ACT
        //        master.Details.Add(new Detail
        //        {
        //            Id = 2
        //        });


        //        //yeOldeObjectContext.DetectChanges();
        //        Assert.IsTrue(GetObjectStateEntries(yeOldeObjectContext).Any(e => e.Entity == master.Details.Single(d => d.Id == 2)), "Detail :O");
        //        Assert.IsTrue(ctx.ChangeTracker.Entries().Any(e => e.Entity == master.Details.Single(d => d.Id == 2)), "Detail not found in ChnageTracker");
        //    }

        //    // ASSERT
        //    //using (var ctx = new IntegrationTestDbContext())
        //    //{
        //    //    var master = ctx.MasterModel.First();
        //    //    Assert.IsNotNull(master);
        //    //    var detail = master.Details.First();
        //    //    Assert.IsNotNull(detail);
        //    //    Assert.IsNotNull(detail.ThirdLevelStoredModel);

        //    //    Assert.AreEqual(detail.ThirdLevelStoredModel.Id, "novaunJoraca");
        //    //}
        //}

        //private static IEnumerable<ObjectStateEntry> GetObjectStateEntries(ObjectContext yeOldeObjectContext)
        //{
        //    return yeOldeObjectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Deleted | EntityState.Modified | EntityState.Unchanged);
        //}
    }

    public class Master
    {
        public int Id { get; set; }
        public virtual List<Detail> Details { get; set; }
    }

    public class Detail
    {
        public int Id { get; set; }
        public int MasterId { get; set; }

        public string ThirdLevelStoredModelData { get; set; }

        [NotMapped] // We need to tell to EF to ignore this property
        [StoredModel("ThirdLevelStoredModelData")]
        public ThirdLevelStoredModel ThirdLevelStoredModel { get; set; }
    }

    public class ThirdLevelStoredModel
    {
        public string Id { get; set; }
    }
}
