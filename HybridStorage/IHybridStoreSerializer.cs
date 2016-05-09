using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HybridStorage
{
    public interface IHybridStoreSerializer
    {
        string Serialize(object entity, Type entityFrameworkObjectType = null);
        object Deserialize(string data, Type objectToDeserializeType, Type entityFrameworkObjectType);
        void Populate(string data, object entity, Type entityFrameworkObjectType);
    }
}
