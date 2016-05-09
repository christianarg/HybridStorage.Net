using System;
namespace HybridStorage
{
    public interface IHybridStore
    {
        IHybridStoreSerializer Serializer { get; set; }
        bool MustProcess(object entity);
        void LoadStoredModels(object entity);
        void StoreStoredModels(object entity);
    }
}
