using System;
using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;
using System.Text;

namespace HybridStorage
{
    public class HybridStoreObjectContext : ObjectContext
    {
        private IHybridStore modelStore;

        public HybridStoreObjectContext(IHybridStore modelStore,string connectionString)
            :base(connectionString)
        {
            this.InitializeHybridStore(modelStore);
        }

        public HybridStoreObjectContext(IHybridStore modelStore, EntityConnection connection, string containerName)
            : base(connection, containerName)
        {
            this.InitializeHybridStore(modelStore);
        }

        private void InitializeHybridStore(IHybridStore theModelStore)
        {
            this.modelStore = theModelStore;
            EFToHybridStore.SetupContext(this, this.modelStore);
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
            this.Refresh(refreshMode, entityList);

            entityList.ForEach((entity) => this.modelStore.LoadStoredModels(entity));
        }
    }
}
