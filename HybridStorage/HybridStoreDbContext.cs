using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;


namespace HybridStorage
{
    public class HybridStoreDbContext : DbContext
    {
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


        public HybridStoreDbContext()
            : base()
        {
            modelStore = new HybridStore();
            ConfigureModelStoreEvents();
        }

        private void ConfigureModelStoreEvents()
        {
            ObjectContext.ObjectMaterialized += ObjectContext_ObjectMaterialized;
            ObjectContext.SavingChanges += ObjectContext_SavingChanges;
        }

        void ObjectContext_SavingChanges(object sender, EventArgs e)
        {
            // TODO: DI
            var modelStore = new HybridStore();

            GetEntriesToProcess()
                .ForEach(entry => modelStore.StoreStoredModels(entry.Entity));
        }

        private List<ObjectStateEntry> GetEntriesToProcess()
        {
            // TODO: Mucho más sencillo con la api de DbEntries del DbContext
            var entries = (from e in ObjectContext.ObjectStateManager.GetObjectStateEntries(
                                        EntityState.Added | EntityState.Modified)
                           where !e.IsRelationship
                           select e).ToList();
            return entries;
        }

        private ObjectContext ObjectContext
        {
            get { return ((IObjectContextAdapter)this).ObjectContext; }
        }

        protected void ObjectContext_ObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
        {
            var entity = e.Entity;
            if (entity == null)
                return;

            // TODO: DI
            var modelStore = new HybridStore();
            modelStore.LoadStoredModels(entity);
        }


    }
}