using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HybridStorage
{
    public class ReflectionHelper
    {
        public static SelfStoredAttribute ReadSelfStoredAttribute(Type type)
        {
            return Attribute.GetCustomAttribute(type, typeof(SelfStoredAttribute)) as SelfStoredAttribute;
        }

        public static StoredModelAttribute ReadStorageAttribute(PropertyInfo sp)
        {
            var storageAttribute = Attribute.GetCustomAttribute(sp, typeof(StoredModelAttribute)) as StoredModelAttribute;
            return storageAttribute;
        }

        public static PropertyInfo[] GetStoredModelProperties(Type entityType)
        {
            return GetPropertyInfo<StoredModelAttribute>(entityType);
        }

        public static PropertyInfo[] GetPropertyInfo<THybridStorageAttribute>(Type entityType)
        {
            var storedModelProperties = entityType.GetProperties()
              .Where(x => Attribute.IsDefined(x, typeof(THybridStorageAttribute), false))
              .ToArray();
            return storedModelProperties;
        }
    }
}
