using System;
using System.Collections.Generic;
using System.Linq;


namespace HybridStorage
{
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
            this.serializer = new NewtonSoftJsonHybridStoreSerializer();
        }

        public void LoadStoredModels(object entity)
        {
            var entityType = entity.GetType();
            var storedModelProperties = GetStoredModelProperties(entityType);
            
            foreach (var smp in storedModelProperties)
            {
                var storageAttribute = GetStorageAttribute(smp); 
                var storagep = entityType.GetProperty(storageAttribute.StorageProperty);
                var value = storagep.GetValue(entity,null);
                if (value != null)
                {
                    // TODO: Ver que pasa cuando la propiedad de la entidad (EF) es una interfaz
                    smp.SetValue(entity, serializer.Deserialize(value.ToString(), smp.PropertyType) , null);
                }
            }
        }

        public void StoreStoredModels(object entity)
        {
            var entityType = entity.GetType();
            var storedModelProperties = GetStoredModelProperties(entityType);

            foreach (var smp in storedModelProperties)
            {
                var storageAttribute = GetStorageAttribute(smp);
                var storedModel = smp.GetValue(entity, null);
                if (storedModel == null)
                    continue;
                // TODO: Ver que pasa cuando la propiedad de la entidad (EF) es una interfaz
                var storagep = entityType.GetProperty(storageAttribute.StorageProperty);
                var serializedModel = serializer.Serialize(storedModel);
                storagep.SetValue(entity, serializedModel, null);
            }
        }

        private static StoredModelAttribute GetStorageAttribute(System.Reflection.PropertyInfo sp)
        {
            var storageAttribute = Attribute.GetCustomAttribute(sp, typeof(StoredModelAttribute)) as StoredModelAttribute;

            return storageAttribute;
        }

        private static System.Reflection.PropertyInfo[] GetStoredModelProperties(Type entityType)
        {
            var storedModelProperties = entityType.GetProperties()
               .Where(x => Attribute.IsDefined(x, typeof(StoredModelAttribute), false))
               .ToArray();
            return storedModelProperties;
        }
    }
}