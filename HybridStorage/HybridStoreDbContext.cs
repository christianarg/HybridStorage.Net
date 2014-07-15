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
            // WARNING!!  Esto es para no obligar a utilizar un contenedor de DI, pero es muy feo!
            modelStore = new HybridStore();
            DisableHybridStore = disableHybridStore;
            if (!disableHybridStore)
                ConfigureModelStoreEvents();

        }

        private void ConfigureModelStoreEvents()
        {
            EFToHybridStore.SetupContext(this.ObjectContext, modelStore);
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

    }
}