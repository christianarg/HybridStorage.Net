using System;
namespace HybridStorage
{
    public interface IHybridStore
    {
        bool MustProcess(object entity);
        void LoadStoredModels(object entity);
        void StoreStoredModels(object entity);
    }
}
