using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HybridStorage
{
    public interface ISelfStoredModel
    {
        void LoadStoredModels();
        bool MustProcess();
        void StoreSelfStoredModel();
    }

    public class SelfStoredModel : ISelfStoredModel
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
        /// Aqui se guarda el objecto serializado
        /// </summary>
        private PropertyInfo storageProperty;
        /// <summary>
        /// Atributo con el que se marca el storedModel que configura donde se guarda (mediante la propiedad StorageProperty)
        /// </summary>
        private SelfStoredAttribute selfStoredAttribute;

        private IHybridStoreSerializer serializer;

        public SelfStoredModel(object entity, SelfStoredAttribute selfStoredAttribute, IHybridStoreSerializer serializer)
        {
            this.entity = entity;
            this.serializer = serializer;
            this.entityType = entity.GetType();
            this.selfStoredAttribute = selfStoredAttribute;
            this.storageProperty = ReflectionHelper.GetPropertyInfoByName(this.entityType, this.selfStoredAttribute.StorageProperty);
        }

        public void LoadStoredModels()
        {
            this.PopulateSelfStoredModel();
        }

        public void StoreSelfStoredModel()
        {
            // TODO: Evitar serializar storage properties normales
            ReflectionHelper.SetValueToProperty(this.storageProperty, this.entity, this.Serialize());
        }

        public bool MustProcess()
        {
            return ReadSelfSerializedModel() != this.Serialize();
        }

        private string Serialize()
        {
            return serializer.Serialize(entity, this.entityType);
        }

        private string ReadSelfSerializedModel()
        {
            return ReflectionHelper.ReadValueFromProperty(this.storageProperty, this.entity) as string;
        }

        private void PopulateSelfStoredModel()
        {
            var selfSerializedModel = ReadSelfSerializedModel();
            if (selfSerializedModel != null)
            {
                serializer.Populate(selfSerializedModel.ToString(), this.entity, this.entityType);
            }
        }
    }

    /// <summary>
    /// Implementación del patron "Null object", para simplficar código cliente (no hay que estar comprobando, null todo el tiempoo)
    /// </summary>
    public class NullSelfStoredObject : ISelfStoredModel
    {
        public void LoadStoredModels()
        {
            // NOOP
        }

        public bool MustProcess()
        {
            return false;
        }

        public void StoreSelfStoredModel()
        {
            // NOOP
        }
    }

    public class SelfStoredModelFactory
    {
        public static ISelfStoredModel CreateSelfStoredModel(object entity, IHybridStoreSerializer serializer)
        {
            var selfStoredAttr = ReflectionHelper.ReadSelfStoredAttribute(entity.GetType());
            if (selfStoredAttr == null)
                return new NullSelfStoredObject();
            return new SelfStoredModel(entity, selfStoredAttr, serializer);
        }
    }
}
