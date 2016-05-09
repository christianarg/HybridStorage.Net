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
        public virtual string Serialize(object objectToSerialize, Type entityFrameworkObjectType = null)
        {
            return JsonConvert.SerializeObject(objectToSerialize, CreateSerializerSettings(objectToSerialize.GetType(), entityFrameworkObjectType));
        }

        public virtual object Deserialize(string data, Type objectToDeserializeType, Type entityFrameworkObjectType)
        {
            return JsonConvert.DeserializeObject(data, objectToDeserializeType, CreateSerializerSettings(objectToDeserializeType, entityFrameworkObjectType));
        }

        public virtual void Populate(string data, object entity, Type entityFrameworkObjectType)
        {
            JsonConvert.PopulateObject(data, entity, CreateSerializerSettings(entityFrameworkObjectType));
        }

        protected virtual JsonSerializerSettings CreateSerializerSettings(Type objectToSerializeOrDeserializeType, Type entityFrameworkObjectType = null)
        {
            if (MustAutomaticallyDetectTypeNameHandlingObjects(objectToSerializeOrDeserializeType))
            {
                var settings = DefaultSerializerSettings();
                settings.TypeNameHandling = TypeNameHandling.Objects;
                return settings;
            }
            return DefaultSerializerSettings();
        }

        /// <summary>
        /// Con esta condición le damos "más inteligencia" al TypeNameHandling = TypeNameHandling.Auto.
        /// 
        /// El problema es que si nuestro Container (objecto EF) tiene una referencia a una clase abstracta a nuestro storedModel, TypeNameHandling = TypeNameHandling.Auto NO serializará el tipo
        /// y al deserializar intentará instanciar una clase abstracta.
        /// 
        /// Con esta condición detectamos al serializar o deserializar clases abstractas y utilizamos TypeNameHandling = TypeNameHandling.Objects.
        /// 
        /// Para cubrir más casos, básicamente hacemos que cualquier tipo que serialicemos cuya clase base no es object utilice TypeNameHandling = TypeNameHandling.Objects.
        /// 
        /// Se puede sobre-escribir y cancelar este comportamiento 
        /// </summary>
        /// <param name="objectToSerializeOrDeserializeType"></param>
        /// <returns></returns>
        protected virtual bool MustAutomaticallyDetectTypeNameHandlingObjects(Type objectToSerializeOrDeserializeType)
        {
            return objectToSerializeOrDeserializeType.IsAbstract || objectToSerializeOrDeserializeType?.BaseType != typeof(object);
        }

        protected virtual JsonSerializerSettings DefaultSerializerSettings()
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,    // Con auto hacemos más eficiente el espacio ocupado por la serialización mientras al mismo tiempo soportamos el caso de serializar herencia
                NullValueHandling = NullValueHandling.Ignore
            };
            return settings;
        }
    }
}
