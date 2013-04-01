using System;
namespace HybridStorage
{
    public interface IHybridStore
    {
        void LoadStoredModels(object entity);
        void StoreStoredModels(object entity);
    }
}
