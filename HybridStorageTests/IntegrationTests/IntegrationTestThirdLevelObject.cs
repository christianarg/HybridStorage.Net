using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
            using (var ctx = new IntegrationTestDbContext())
            {
                var arr = new Master(){ Id=1 };
                arr.Details = new List<Detail>(){ new Detail(){Id = 1, ThirdLevelStoredModel = new ThirdLevelStoredModel(){Id = "novaunJoraca"}}};

                ctx.MasterModel.Add(arr);
                ctx.SaveChanges();
            }

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
