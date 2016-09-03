using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HybridStoragePerformanceTester.PerformanceTest
{
    public class RandomDataGenerator
    {
        private static Random random = new Random((int)DateTime.Now.Ticks);

        public static List<string> Languages = new List<string>
        {
            "es-ES",
            "ca-ES",
            "en-US",
            "fr-FR"
        };

        public static int RandomNumber(int maxSize)
        {
            return RandomDataGenerator.random.Next(maxSize) + 1;
        }

        public static string RandomString(int size)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                char value = Convert.ToChar(Convert.ToInt32(Math.Floor(26.0 * RandomDataGenerator.random.NextDouble() + 65.0)));
                stringBuilder.Append(value);
            }
            return stringBuilder.ToString();
        }

        public static string RandomLanguage()
        {
            return RandomDataGenerator.Languages[RandomDataGenerator.random.Next(4)];
        }

        public static RandomContent RandomContent()
        {
            var hybridContent = new RandomContent
            {
                Id = RandomDataGenerator.RandomString(30),
                Language = RandomDataGenerator.RandomLanguage()
            };
            for (int i = 0; i < 20; i++)
            {
                hybridContent.Fields.Add(new RandomField
                {
                    Name = RandomDataGenerator.RandomString(30),
                    Value = RandomDataGenerator.RandomString(2000)
                });
            }
            return hybridContent;
        }
    }
}
