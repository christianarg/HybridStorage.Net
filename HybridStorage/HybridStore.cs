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
            this.CreateSelfStoredModel(entity).LoadStoredModels();
        }

        public void StoreStoredModels(object entity)
        {
            this.CreateStoredModels(entity).ForEach(sm => sm.StoreStoredModel());
            CreateSelfStoredModel(entity).StoreSelfStoredModel();
        }

        public bool MustProcess(object entity)
        {
            return CreateSelfStoredModel(entity).MustProcess()           
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

        private ISelfStoredModel CreateSelfStoredModel(object entity)
        {
            return SelfStoredModelFactory.CreateSelfStoredModel(entity, this.serializer);
        }

        private bool MustProcessStoredModel(object entity)
        {
            return this.CreateStoredModels(entity).Any(sm => sm.MustProcess());
        }
    }
}