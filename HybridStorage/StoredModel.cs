using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HybridStorage
{
    /// <summary>
    /// Representa un modelo persistido y metodos para manipularlo
    /// </summary>
    public class StoredModel
    {
        /// <summary>
        /// La entidad de EF que contiene nuestro Stored model
        /// </summary>
        private object entity;
        /// <summary>
        /// Tipo de la entidad EF
        /// </summary>
        private Type entityType;
        /// <summary>
        /// Propiedad donde se "asigna" nuestro objeto serializado
        /// </summary>
        private PropertyInfo storedModelProperty;
        /// <summary>
        /// Atributo con el que se marca el storedModel que configura donde se guarda (mediante la propiedad StorageProperty)
        /// y a la vez notifica que en que propiedad se debe deserializar (es la propiedad que el atributo decora)
        /// </summary>
        private StoredModelAttribute storedModelAttribute;
        /// <summary>
        /// Aqui se guarda el objecto serializado
        /// </summary>
        private PropertyInfo storageProperty;

        private IHybridStoreSerializer serializer;

        public StoredModel(object entity, PropertyInfo storedModelProperty, IHybridStoreSerializer serializer)
        {
            this.entity = entity;
            this.entityType = entity.GetType();
            this.storedModelProperty = storedModelProperty;
            this.storedModelAttribute = ReflectionHelper.ReadStorageAttribute(this.storedModelProperty);
            this.storageProperty = this.entityType.GetProperty(this.storedModelAttribute.StorageProperty);
            this.serializer = serializer;
        }

        public void LoadStoredModel()
        {
            this.AssignModelReferenceToStoredModelProperty();
        }

        public void StoreStoredModel()
        {
            var storedModel = this.ReadStoredModel();
            if (storedModel == null)
            {
                // "Nuleamos" el almacenamiento en caso de que el modelo sea nulo
                this.AssignValueToStorageProperty(serializedValue: null);
                return;
            }

            this.AssignValueToStorageProperty(serializedValue: this.serializer.Serialize(storedModel));

            this.ProcessInheritanceContained(storedModel);
        }

        public bool MustProcess()
        {
            var storedModel = this.ReadStoredModel();
            var newSerializedModel = storedModel != null ? serializer.Serialize(storedModel) : null;
            // Comprar la serialización. Si es distinta es que ha cambiado y hay que procesar
            return ReadSerializedModelFromStorageProperty() != newSerializedModel;
        }

        private void ProcessInheritanceContained(object storedModel)
        {
            if (ReflectionHelper.HasAttribute<InheritanceContained>(this.storedModelProperty))
            {
                // TODO: Detectar Mappings y utilizar Map "normal"
                Mapper.DynamicMap(storedModel, entity, storedModel.GetType(), entityType);
            }
        }

        private void AssignValueToStorageProperty(string serializedValue)
        {
            ReflectionHelper.SetValueToProperty(this.storageProperty, this.entity, valueToSet: serializedValue);
        }

        private void AssignModelReferenceToStoredModelProperty()
        {
            ReflectionHelper.SetValueToProperty(this.storedModelProperty, this.entity, DeserializeModel());
        }

        private object ReadStoredModel()
        {
            return ReflectionHelper.ReadValueFromProperty(this.storedModelProperty, this.entity);
        }

        private string ReadSerializedModelFromStorageProperty()
        {
            return ReflectionHelper.ReadValueFromProperty(this.storageProperty, this.entity) as string;
        }

        private object DeserializeModel()
        {
            // TODO: Ver que pasa cuando la propiedad de la entidad (EF) es una interfaz
            var serializedModel = this.ReadSerializedModelFromStorageProperty();
            if(serializedModel == null)
                return null;
            var deserializedModel = serializer.Deserialize(serializedModel, storedModelProperty.PropertyType);
            return deserializedModel;
        }
    }

    /// <summary>
    /// Factory class en la que creamos las abstracciones Stored Model
    /// para realizar leer, guardar o comprobar si se debe procesar
    /// </summary>
    public class StoredModelFactory
    {

        public static List<StoredModel> CreateStoredModels(object entity, IHybridStoreSerializer serializer)
        {
            var result = new List<StoredModel>();
            var entityType = entity.GetType();

            foreach (var storedModelProperty in ReflectionHelper.GetStoredModelProperties(entityType))
            {
                result.Add(new StoredModel(entity, storedModelProperty, serializer));
            }

            return result;
        }
    }
}
