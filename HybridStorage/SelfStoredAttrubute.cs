using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HybridStorage
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class SelfStoredAttribute : Attribute
    {
        /// <summary>
        /// The property that will contain the serialized model
        /// </summary>
        public string StorageProperty { get; private set; }
        
        /// <summary>
        /// Used to "mark" the property that will contain a stored model
        /// </summary>
        /// <param name="storageProperty">The property that will contain the serialized model</param>
        public SelfStoredAttribute(string storageProperty)
        {
            this.StorageProperty = storageProperty;
        }
    }
}
