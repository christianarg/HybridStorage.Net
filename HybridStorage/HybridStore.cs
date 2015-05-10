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
            this.CreateStoredModels(entity).ForEach(sm => sm.LoadStoredModel());

            LoadSelfStoredModels(entity, entity.GetType());
        }

        public void StoreStoredModels(object entity)
        {
            this.CreateStoredModels(entity).ForEach(sm => sm.StoreStoredModel());

            StoreSelfStoredModel(entity, entity.GetType());
        }

        public bool MustProcess(object entity)
        {
            var entityType = entity.GetType();

            return ReflectionHelper.ReadSelfStoredAttribute(entityType) != null
                 || this.MustProcessStoredModel(entity);
        }

        /// <summary>
        /// Factory method en la que creamos las abstracciones Stored Model
        /// para realizar leer, guardar o comprobar si se debe procesar
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private List<StoredModel> CreateStoredModels(object entity)
        {
            return StoredModelFactory.CreateStoredModels(entity, this.serializer);
        }

        private bool MustProcessStoredModel(object entity)
        {
            return this.CreateStoredModels(entity).Any(sm => sm.MustProcess());
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
}