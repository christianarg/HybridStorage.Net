using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HybridStorage
{
    /// <summary>
    /// Clase completamente opcional para utilizar como clase base cuando creamos
    /// una clase "contenedora" de una jerarquía de herencia
    /// Nos "mapea" automáticamente la propiedad Id de la entidad "contenida" en la entidad
    /// "contenedora" si ambos tienen esta propiedad
    /// </summary>
    public abstract class HybridEntityContainerBase
    {
        internal HybridEntityContainerBase() { } // Necesario EF y testing

        protected HybridEntityContainerBase(object realPocoEntity)
        {
            var containerIdProperty = GetProperty(this, "Id");
            if (containerIdProperty == null)
                return; ;
            var realPocoId = GetPropertyValue(realPocoEntity, "Id");
            if (realPocoId == null)
                return;
            containerIdProperty.SetValue(this, realPocoId, null);
        }

        private object GetPropertyValue(object @object, string propertyId)
        {
            var propertyInfo = GetProperty(@object, propertyId);
            if (propertyInfo == null)
                return null;
            var id = propertyInfo.GetValue(@object, null);
            return id;
        }

        private PropertyInfo GetProperty(object @object, string propertyId)
        {
            var entityType = @object.GetType();
            var propertyType = entityType.GetProperties().SingleOrDefault(p => p.Name == propertyId);
            if (propertyType == null)
                return null;
            return propertyType;
        }
    }

}
