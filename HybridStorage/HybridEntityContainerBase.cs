using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HybridStorage
{
    internal interface IHybHybridEntityContainer
    {
        //object Model { get; }
        void SetModel(object model);
    }
    /// <summary>
    /// clase base cuando creamos
    /// una clase "contenedora" de una jerarquía de herencia
    /// Nos "mapea" automáticamente la propiedad Id de la entidad "contenida" en la entidad
    /// "contenedora" si ambos tienen esta propiedad
    /// </summary>
    public abstract class HybridEntityContainerBase<TRealPocoEntity> : IHybHybridEntityContainer
        where TRealPocoEntity:class
    {
        internal HybridEntityContainerBase() { } // Necesario EF y testing

        public string Data { get; set; }

        [StoredModel("Data")]
        [InheritanceContained]
        public TRealPocoEntity Model { get; set; }


        void IHybHybridEntityContainer.SetModel(object model)
        {
            this.SetModel(model);
        }

        void SetModel(object model)
        {
            var castedModel = model as TRealPocoEntity;
            if (castedModel == null)
                throw new Exception("Tipo incorrecto");
            Model = castedModel;

            // TODO: ofrecer posibilidad de "map" normal
            // Rellenamos las propiedades del container. Estas son las que podremos "queriar"
            Mapper.DynamicMap(castedModel, this, castedModel.GetType(), this.GetType());
        }

        protected HybridEntityContainerBase(TRealPocoEntity realPocoEntity)
        {
            SetModel(realPocoEntity);
        }

        public T Cast<T>()
            where T : class, TRealPocoEntity
        {
            return Model as T;
        }
    }
}
