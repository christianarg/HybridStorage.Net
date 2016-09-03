using Fclp;
using HybridStoragePerformanceTester.HybridModel;
using HybridStoragePerformanceTester.NormalModel;
using HybridStoragePerformanceTester.PerformanceTest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HybridStoragePerformanceTester
{
    class Program
    {
        static HybridModelTestApi hybridTestApi;
        static NormalModelTestApi normalModelTestApi;

        static List<IPerformanceTestApi> performanceTestApis = new List<IPerformanceTestApi>();

        static void Main(string[] args)
        {
            CreateTestApiInstances();

            switch (args[0])
            {
                case "-i":
                    Init();
                    break;
                case "-t":
                    Test();
                    break;
            }

        }

        private static void CreateTestApiInstances()
        {
            hybridTestApi = new HybridModelTestApi();
            normalModelTestApi = new NormalModelTestApi();
            performanceTestApis.Add(normalModelTestApi);
            performanceTestApis.Add(hybridTestApi);
        }

        private static void Init()
        {
            ExecuteForAllApis((api) =>
            {
                Console.WriteLine($"Cleaning test for: {ApiName(api)}");
                api.Clean();
            });

            const int maxContents = 300;

            for (int i = 0; i < maxContents; i++)
            {
                var randomContent = RandomDataGenerator.RandomContent();

                ExecuteForAllApis((api) =>
                {
                    Console.WriteLine($"Initializing data for: {ApiName(api)}; count: {i} ");

                    api.CreateRandomContent(randomContent);
                });
            }

        }

        private static void Test()
        {
            ExecuteForAllApis((api) =>
            {
                Console.WriteLine($"Executing test for: {ApiName(api)}");
                // query to avoid cold query interfere with results
                var firstId = api.GetAllIds().First();
                api.FindById(firstId);
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                api.GetAllIds().ForEach(id => api.FindById(id));
                stopWatch.Stop();
                Console.WriteLine($"Elapsed: {stopWatch.ElapsedMilliseconds}");
            });

            Console.WriteLine("Test completed. Press any key to continue...");
            Console.ReadLine();
        }

        private static string ApiName(IPerformanceTestApi api)
        {
            return api.GetType().Name;
        }

        private static void ExecuteForAllApis(Action<IPerformanceTestApi> action)
        {
            foreach (var api in performanceTestApis)
            {
                action(api);
            }
        }
    }
}
