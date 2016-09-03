using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HybridStoragePerformanceTester.PerformanceTest
{
    public class RandomContent
    {
        public string Id { get; set; }

        public string Language { get; set; }

        public List<RandomField> Fields { get; set; }

        public RandomContent()
        {
            this.Fields = new List<RandomField>();
        }
    }

    public class RandomField
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}
