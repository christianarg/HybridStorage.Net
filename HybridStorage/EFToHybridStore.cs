using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Objects;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Text;

namespace HybridStorage
{
    /// <summary>
    /// "Conector" EF con hybrid store
    /// Inicializa los eventos de EF para que se comunique con HybridStore al persistir y al recuperar entidades
    /// 
    /// Funciona tanto para EF4 como para EF5
    /// </summary>
    public class EFToHybridStore
    {
        readonly ObjectContext context;
        readonly IHybridStore modelStore;

        public event EntriesProcessedEventHander EntriesProcessed;

        public EFToHybridStore(ObjectContext context, IHybridStore modelStore)
        {
            this.context = context;
            this.modelStore = modelStore;
            context.ObjectMaterialized += ObjectContext_ObjectMaterialized;
            context.SavingChanges += ObjectContext_SavingChanges;
        }

        public static EFToHybridStore SetupContext(ObjectContext context, IHybridStore modelStore)
        {
            return new EFToHybridStore(context, modelStore);
        }
       
        protected void ObjectContext_ObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
        {
            var entity = e.Entity;
            if (entity == null)
                return;

            modelStore.LoadStoredModels(entity);
        }
   
        void ObjectContext_SavingChanges(object sender, EventArgs e)
        {
            var entities = ProcessEntries();
            if (this.EntriesProcessed != null)
                EntriesProcessed(this, new EntriesProcessedEvent(entities));
        }

        private List<object> ProcessEntries()
        {
            var processedEntities = new List<object>();
            // Este DetectChanges es MUY importante si HybridStore se utiliza desde EF4
            // Hay casos cuando tenemos una relación "master / detail" que al añadir el detail al master
            // el ObjectStateManager no tiene constancia de ello hasta este detectChanges
            // Si el Detail tiene HybridStore, no guardaría correctamente los datos
            context.DetectChanges();
            var entries = context.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified | EntityState.Unchanged).ToList();
            foreach (var entry in entries)
            {
                if (entry.Entity != null && modelStore.MustProcess(entry.Entity))
                {
                    processedEntities.Add(entry.Entity);
                    // Procesar. Se podría hacer más optimo comparando si el valor serializado ha cambiado
                    modelStore.StoreStoredModels(entry.Entity);
                    // Avisarle a EF que ha cambiado
                    if (entry.State == EntityState.Unchanged)
                    {
                        entry.ChangeState(EntityState.Modified);
                    }
                }
            }
            return processedEntities;
        }

        public delegate void EntriesProcessedEventHander(object sender, EntriesProcessedEvent args);

        public class EntriesProcessedEvent : EventArgs 
        {
            public List<object> Entities { get; set; }

            public EntriesProcessedEvent(List<object> entities)
            {
                this.Entities = entities;
            }
        }
    }
}
