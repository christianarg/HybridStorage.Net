using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Text;

namespace HybridStorage
{
    public class HybridStoreObjectContext : ObjectContext
    {
        public HybridStoreObjectContext(IHybridStore modelStore,string connectionString)
            :base(connectionString)
        {
            EFToHybridStore.SetupContext(this, modelStore);
        }
    }
}
