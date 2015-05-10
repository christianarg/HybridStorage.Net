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

        public static bool ClassHasAttribute<THybridStorageAttribute>(object @object)
        {
            return Attribute.IsDefined(@object.GetType(), typeof(InheritanceContained));
        }

        public static bool HasAttribute<THybridStorageAttribute>(PropertyInfo info)
        {
            return Attribute.IsDefined(info, typeof(THybridStorageAttribute), false);
        }

        public static PropertyInfo[] GetPropertyInfo<THybridStorageAttribute>(Type entityType)
        {
            var storedModelProperties = entityType.GetProperties()
                .Where(x => Attribute.IsDefined(x, typeof(THybridStorageAttribute), inherit: false))
                .ToArray();
            return storedModelProperties;
        }

        public static object ReadValueFromProperty(PropertyInfo propertyInfoToRead, object objectThatOwnsTheProperty)
        {
            var propertyValue = propertyInfoToRead.GetValue(objectThatOwnsTheProperty, index: null);
            return propertyValue;
        }

        public static void SetValueToProperty(PropertyInfo propertyInfoToSet, object objectThatOwnsTheProperty, object valueToSet)
        {
            propertyInfoToSet.SetValue(objectThatOwnsTheProperty, valueToSet, index: null);
        }

        public static PropertyInfo GetPropertyInfoByName(Type entityType, string propertyName)
        {
            return entityType.GetProperty(propertyName);
        }
    }
}
