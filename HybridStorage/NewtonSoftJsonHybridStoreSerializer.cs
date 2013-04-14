using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HybridStorage
{
    /// <summary>
    /// Serializador por defecto y recomendado basado totalmente en la excelente librería NewtonSoft Json.Net
    /// </summary>
    public class NewtonSoftJsonHybridStoreSerializer : IHybridStoreSerializer
    {
        public string Serialize(object entity)
        {
            return JsonConvert.SerializeObject(entity, CreateSerializerSettings());
        }

        public object Deserialize(string data, Type type)
        {
            return JsonConvert.DeserializeObject(data, type, CreateSerializerSettings());
        }

        private static JsonSerializerSettings CreateSerializerSettings()
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects
            };
            return settings;
        }


        public void Populate(string data, object entity)
        {
            JsonConvert.PopulateObject(data, entity);
        }
    }
}
