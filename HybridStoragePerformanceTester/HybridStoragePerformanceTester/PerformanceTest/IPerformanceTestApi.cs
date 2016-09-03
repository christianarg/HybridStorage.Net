using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HybridStoragePerformanceTester.PerformanceTest
{
    public interface IPerformanceTestApi
    {
        void Clean();
        List<string> GetAllIds();
        int ContentsCount();
        void CreateRandomContent(RandomContent randomContent);
        void FindById(string id);
        void FindByIdAsNoTracking(string id);
    }
}
