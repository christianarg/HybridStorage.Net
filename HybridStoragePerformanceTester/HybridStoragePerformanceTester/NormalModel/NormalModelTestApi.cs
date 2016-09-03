using AutoMapper;
using HybridPerformanceTests.NormalModel;
using HybridStoragePerformanceTester.PerformanceTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HybridStoragePerformanceTester.NormalModel
{
    public class NormalModelTestApi : IPerformanceTestApi
    {
        public void Clean()
        {
            using (NormalDbContextDbContext normalDbContext = CreateContext())
            {
                normalDbContext.Database.ExecuteSqlCommand("delete from NormalFields", new object[0]);
                normalDbContext.Database.ExecuteSqlCommand("delete from NormalContents", new object[0]);
            }
        }

        public List<string> GetAllIds()
        {
            List<string> result = new List<string>();
            using (NormalDbContextDbContext normalDbContext = CreateContext())
            {
                string connectionString = normalDbContext.Database.Connection.ConnectionString;
                result = (from nc in normalDbContext.NormalContents
                          select nc.Id).ToList<string>();
            }
            return result;
        }

        public int ContentsCount()
        {
            return GetAllIds().Count;
        }

        public void CreateRandomContent(RandomContent randomContent)
        {
            using (NormalDbContextDbContext normalDbContext = CreateContext())
            {
                normalDbContext.NormalContents.Add(Mapper.DynamicMap<NormalContent>(randomContent));

                normalDbContext.SaveChanges();
            }
        }

        public void FindById(string id)
        {
            using (NormalDbContextDbContext normalDbContext = CreateContext())
            {
                NormalContent normalContent = normalDbContext.NormalContents.Include("Fields").SingleOrDefault((NormalContent nc) => nc.Id == id);
            }
        }

        public void FindByIdAsNoTracking(string id)
        {
            using (NormalDbContextDbContext normalDbContext = CreateContext())
            {
                NormalContent normalContent = normalDbContext.NormalContents.AsNoTracking().Include("Fields").SingleOrDefault((NormalContent nc) => nc.Id == id);
            }
        }

        public NormalContent RandomContent()
        {
            NormalContent normalContent = new NormalContent
            {
                Id = RandomDataGenerator.RandomString(30),
                Language = RandomDataGenerator.RandomLanguage()
            };
            for (int i = 0; i < 20; i++)
            {
                normalContent.Fields.Add(new NormalField
                {
                    Name = RandomDataGenerator.RandomString(30),
                    Value = RandomDataGenerator.RandomString(2000)
                });
            }
            return normalContent;
        }

        private NormalDbContextDbContext CreateContext()
        {
            return new NormalDbContextDbContext();
        }
    }
}
