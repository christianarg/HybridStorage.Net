using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace HybridStorage
{
    /// <summary>
    /// Used to "mark" the property that will contain a stored model
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class StoredModelAttribute : NotMappedAttribute
    {
        /// <summary>
        /// The property that will contain the serialized model
        /// </summary>
        public string StorageProperty { get; private set; }
        
        /// <summary>
        /// Used to "mark" the property that will contain a stored model
        /// </summary>
        /// <param name="storageProperty">The property that will contain the serialized model</param>
        public StoredModelAttribute(string storageProperty)
        {
            this.StorageProperty = storageProperty;
        }
    }
}