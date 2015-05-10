using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;


namespace HybridStorage
{
    /// <summary>
    /// Se engarga de reconstituir los objetos serializados en la consulta
    /// y serializar los objetos en el guardado
    /// </summary>
    public class HybridStore : IHybridStore
    {
        IHybridStoreSerializer serializer;

        public HybridStore(IHybridStoreSerializer serializer)
        {
            this.serializer = serializer;
        }

        public HybridStore()
        {
            // Para que quien no quiera utilizar un contenedor de DI y pueda utilizar ModelStore fácilmente
            // "trade off", ya que acoplamos fuertemente
            this.serializer = new NewtonSoftJsonHybridStoreSerializer();
        }

        public void LoadStoredModels(object entity)
        {
            var entityType = entity.GetType();

            LoadStoredModels(entity, entityType);  // TODO: Buscar un nombre mejor
            LoadSelfStoredModels(entity, entityType);
        }

        public void StoreStoredModels(object entity)
        {
            var entityType = entity.GetType();
            StoreStoredModels(entity, entityType);

            StoreSelfStoredModel(entity, entityType);
        }

        public bool MustProcess(object entity)
        {
            var entityType = entity.GetType();

            return ReflectionHelper.ReadSelfStoredAttribute(entityType) != null
                 || this.MustProcessStoredModel(entity);
            //    || ReflectionHelper.GetStoredModelProperties(entityType).Length > 0;
        }

        private bool MustProcessStoredModel(object entity)
        {
            var entityType = entity.GetType();
            var storedModelProperties = ReflectionHelper.GetStoredModelProperties(entityType);

            foreach (var smp in storedModelProperties)
            {
                // Obtener propiedad string donde se almacenan los datos
                var storageAttribute = ReflectionHelper.ReadStorageAttribute(smp);
                var storageProperty = entityType.GetProperty(storageAttribute.StorageProperty);
                var originalSerializedModel = storageProperty.GetValue(entity, null) as string;

                // Obtener la referencia al modelo de la propiedad marcada como StoredModel
                var storedModel = smp.GetValue(entity, null);

                // Comprar la serialización. Si es distinta es que ha cambiado y hay que procesar
                var newSerializedModel = storedModel != null ? serializer.Serialize(storedModel) : null;

                bool hasChanged = originalSerializedModel != newSerializedModel;
                if (hasChanged)
                    return true;
            }
            return false;
        }


        private void StoreSelfStoredModel(object entity, Type entityType)
        {
            var selfStoredAttr = ReflectionHelper.ReadSelfStoredAttribute(entityType);
            if (selfStoredAttr == null)
                return;

            // TODO: Evitar serializar storage properties normales
            var selfSerializedModel = serializer.Serialize(entity);
            // asignar a sef storage property
            var selfStorageProperty = entityType.GetProperty(selfStoredAttr.StorageProperty);
            selfStorageProperty.SetValue(entity, selfSerializedModel, null);
        }

        private void StoreStoredModels(object entity, Type entityType)
        {
            var storedModelProperties = ReflectionHelper.GetStoredModelProperties(entityType);

            foreach (var smp in storedModelProperties)
            {
                // Obtener propiedad string donde se almacenan los datos
                var storageAttribute = ReflectionHelper.ReadStorageAttribute(smp);
                var storageProperty = entityType.GetProperty(storageAttribute.StorageProperty);


                // Obtener la referencia al modelo de la propiedad marcada como StoredModel
                var storedModel = smp.GetValue(entity, null);
                if (storedModel == null)
                {
                    // "Nuleamos" el almacenamiento en caso de que el modelo sea nulo
                    storageProperty.SetValue(entity, null, null);
                    continue;
                }

                // TODO: Ver que pasa cuando la propiedad de la entidad (EF) es una interfaz
                // Serializar el modelo en la propiedad en la StorageProperty
                var serializedModel = serializer.Serialize(storedModel);

                // Asignar valor serializado al Storage Property
                storageProperty.SetValue(entity, serializedModel, null);

                if (ReflectionHelper.HasAttribute<InheritanceContained>(smp))
                {
                    // TODO: Detectar Mappings y utilizar Map "normal"
                    Mapper.DynamicMap(storedModel, entity, storedModel.GetType(), entityType);
                }
            }
        }

        private void LoadStoredModels(object entity, Type entityType)
        {
            foreach (var storedModelProperty in ReflectionHelper.GetStoredModelProperties(entityType))
            {
                var serializedObject = ReadSerializedModelFromStorageProperty(entity, entityType, storedModelProperty);

                if (serializedObject == null)
                    continue;

                var deserializedModel = DeserializeModel(storedModelProperty, serializedObject);

                AssignModelReferenceToStoredModelProperty(entity, storedModelProperty, deserializedModel);
            }
        }

        private static void AssignModelReferenceToStoredModelProperty(object entity, PropertyInfo storedModelProperty, object deserializedModel)
        {
            storedModelProperty.SetValue(entity, deserializedModel, null);
        }

        private object DeserializeModel(PropertyInfo storedModelProperty, object serializedObject)
        {
            // TODO: Ver que pasa cuando la propiedad de la entidad (EF) es una interfaz
            var deserializedModel = serializer.Deserialize(serializedObject.ToString(), storedModelProperty.PropertyType);
            return deserializedModel;
        }



        private static object ReadSerializedModelFromStorageProperty(object entity, Type entityType, PropertyInfo storedModelProperty)
        {
            var storageAttribute = ReflectionHelper.ReadStorageAttribute(storedModelProperty);
            var storagep = entityType.GetProperty(storageAttribute.StorageProperty);
            var value = storagep.GetValue(entity, null);
            return value;
        }

        private void LoadSelfStoredModels(object entity, Type entityType)
        {
            var selfSerializedModel = ReadSelfSerializedModel(entity, entityType);
            PopulateSelfStoredModel(entity, selfSerializedModel);
        }

        private object ReadSelfSerializedModel(object entity, Type entityType)
        {
            var selfStoredAttr = ReflectionHelper.ReadSelfStoredAttribute(entityType);
            if (selfStoredAttr == null)
                return null;
            var selfStorageProperty = entityType.GetProperty(selfStoredAttr.StorageProperty);
            return selfStorageProperty.GetValue(entity, null);
        }

        private void PopulateSelfStoredModel(object entity, object selfSerializedModel)
        {
            if (selfSerializedModel != null)
            {
                serializer.Populate(selfSerializedModel.ToString(), entity);
            }
        }
    }

    //public 
}