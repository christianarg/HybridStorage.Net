using AutoMapper;
using HybridPerformanceTests.Hybrid;
using HybridStoragePerformanceTester.PerformanceTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HybridStoragePerformanceTester.HybridModel
{
    public class HybridModelTestApi : IPerformanceTestApi
    {
        public void Clean()
        {
            using (HybridDbContextDbContext hybridDbContext = new HybridDbContextDbContext())
            {
                hybridDbContext.Database.ExecuteSqlCommand("delete from HybridContents", new object[0]);
            }
        }

        public List<string> GetAllIds()
        {
            List<string> result = new List<string>();
            using (HybridDbContextDbContext hybridDbContext = new HybridDbContextDbContext())
            {
                result = (from nc in hybridDbContext.HybridContents
                          select nc.Id).ToList<string>();
            }
            return result;
        }

        public int ContentsCount() => GetAllIds().Count;

        public void CreateRandomContent(RandomContent randomContent)
        {
            using (HybridDbContextDbContext hybridDbContext = new HybridDbContextDbContext())
            {
                hybridDbContext.HybridContents.Add(Mapper.DynamicMap<HybridContent>(randomContent));

                hybridDbContext.SaveChanges();
            }
        }

        public void FindById(string id)
        {
            using (HybridDbContextDbContext hybridDbContext = new HybridDbContextDbContext())
            {
                HybridContent hybridContent = hybridDbContext.HybridContents.SingleOrDefault((HybridContent c) => c.Id == id);
            }
        }

        public void FindByIdAsNoTracking(string id)
        {
            using (HybridDbContextDbContext hybridDbContext = new HybridDbContextDbContext())
            {
                HybridContent hybridContent = hybridDbContext.HybridContents.AsNoTracking().SingleOrDefault((HybridContent c) => c.Id == id);
            }
        }

        //public HybridContent RandomContent()
        //{
        //    HybridContent hybridContent = new HybridContent
        //    {
        //        Id = RandomDataGenerator.RandomString(30),
        //        Language = RandomDataGenerator.RandomLanguage()
        //    };
        //    for (int i = 0; i < 20; i++)
        //    {
        //        hybridContent.Fields.Add(new HybridField
        //        {
        //            Name = RandomDataGenerator.RandomString(30),
        //            Value = RandomDataGenerator.RandomString(2000)
        //        });
        //    }
        //    return hybridContent;
        //}
    }
}
