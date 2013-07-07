using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace HybridStorage
{
    public static class HybridDbSetExtensions
    {
        public static void HybridAdd<TEntityContainer>(this DbSet<TEntityContainer> dbSet, Object entity)
            where TEntityContainer : class, new()
        {
            var type = typeof(TEntityContainer);
            if (typeof(IHybHybridEntityContainer).IsAssignableFrom(type))
            {
                var container = new TEntityContainer();
                ((IHybHybridEntityContainer)container).SetModel(entity);
                dbSet.Add(container);
            }
            else
            {
                dbSet.Add(entity as TEntityContainer);
            }
        }

        public static TEntity HybridFind<TEntityContainer, TEntity>(this DbSet<TEntityContainer> dbSet, params object[] keyValues)
            where TEntity : class
            where TEntityContainer : class
        {
            // dynamic como "shortcut" de reflection
            dynamic contentContainer = dbSet.Find(keyValues) as IHybHybridEntityContainer;
            if (contentContainer == null)
                return null;
            return contentContainer.Model as TEntity;
        }

        public static void Remove<TEntityContainer, TEntity>(this DbSet<TEntityContainer> dbSet, TEntity entity)
            where TEntity : class
            where TEntityContainer : class
        {
            dynamic realpoco = entity;
            var contentContainer = dbSet.Find(realpoco.Id);
            if (contentContainer == null)
                return; // O excepción no se
            dbSet.Remove(contentContainer);
        }
    }
}
