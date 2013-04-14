using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HybridStorage
{
    public interface IHybridStoreSerializer
    {
        string Serialize(object entity);
        object Deserialize(string data, Type type);

        void Populate(string data, object entity);
    }
}
