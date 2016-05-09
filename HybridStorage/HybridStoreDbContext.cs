using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;


namespace HybridStorage
{
    /// <summary>
    /// https://github.com/christianarg/HybridStorage.Net
    /// </summary>
    public abstract class HybridStoreDbContext : DbContext
    {
        /// <summary>
        /// Propiedad utilizada para evitar "explosiones" durante pruebas de integración
        /// </summary>
        public bool DisableHybridStore { get; set; }
        IHybridStore modelStore;
        public EFToHybridStore efToHybridStore { get; internal set; }

        public IHybridStoreSerializer Serializer
        {
            get { return modelStore.Serializer; }
            set { modelStore.Serializer = value; }
        }

        public HybridStoreDbContext(IHybridStore modelStore, string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            this.modelStore = modelStore;
            ConfigureModelStoreEvents();
        }

        public HybridStoreDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            modelStore = new HybridStore();
            ConfigureModelStoreEvents();
        }

        public HybridStoreDbContext(IHybridStore modelStore)
        {
            this.modelStore = modelStore;
            ConfigureModelStoreEvents();
        }


        public HybridStoreDbContext(bool disableHybridStore = false)
            : base()
        {
            // WARNING!!  Esto es para no obligar a utilizar un contenedor de DI, pero es muy feo!
            modelStore = new HybridStore();
            DisableHybridStore = disableHybridStore;
            if (!disableHybridStore)
                ConfigureModelStoreEvents();

        }

        private void ConfigureModelStoreEvents()
        {
            this.efToHybridStore = EFToHybridStore.SetupContext(this.ObjectContext, modelStore);
        }

        private ObjectContext ObjectContext
        {
            get
            {

                try
                {
                    return ((IObjectContextAdapter)this).ObjectContext;
                }
                catch (Exception)
                {
                    if (!DisableHybridStore)
                        throw;
                    return null;
                }
            }
        }

        /// <summary>
        /// Refresh del contexto teniendo en cuenta StoredModels
        /// ObjectContext tiene el método Refresh que NO tiene eventos ni es posible sobre escribirlo
        /// De momento la manera de refrescar el contexto y que se enteren los Stored Models
        /// es hacer esta llamada especial que llama al Refresh pero aparte carga los modelos guardados
        /// </summary>
        /// <param name="refreshMode"></param>
        /// <param name="entities"></param>
        public void HybridRefresh(RefreshMode refreshMode, IEnumerable<object> entities)
        {
            var entityList = entities.ToList();
            ObjectContext.Refresh(refreshMode, entityList);

            entityList.ForEach((entity) => this.modelStore.LoadStoredModels(entity));
        }

    }
}