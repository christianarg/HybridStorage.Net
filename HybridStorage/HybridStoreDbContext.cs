using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
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
            modelStore = new HybridStore();
            DisableHybridStore = disableHybridStore;
            if(!disableHybridStore)
                ConfigureModelStoreEvents();
        }

        private void ConfigureModelStoreEvents()
        {
            ObjectContext.ObjectMaterialized += ObjectContext_ObjectMaterialized;
            ObjectContext.SavingChanges += ObjectContext_SavingChanges;
        }

        void ObjectContext_SavingChanges(object sender, EventArgs e)
        {
            GetEntriesToProcess()
                .ForEach(ProcessDbEntry);
        }
        void ProcessDbEntry(DbEntityEntry entry)
        {
            // Si es un storedModel
            if (modelStore.MustProcess(entry.Entity))
            {
                // Procesar. Se podría hacer más optimo comparando si el valor serializado ha cambiado
                modelStore.StoreStoredModels(entry.Entity);

                // Avisarle a EF que ha cambiado
                if (entry.State == EntityState.Unchanged)
                    entry.State = EntityState.Modified;

            }
        }

        private List<DbEntityEntry> GetEntriesToProcess()
        {
            return ChangeTracker.Entries().ToList();
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
                    if(!DisableHybridStore)
                        throw;
                    return null;
                }
            }
        }

        protected void ObjectContext_ObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
        {
            var entity = e.Entity;
            if (entity == null)
                return;

            modelStore.LoadStoredModels(entity);
        }
    }
}