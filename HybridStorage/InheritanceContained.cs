using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HybridStorage
{
    /// <summary>
    /// Used to "mark" the property so that it copies values from convention 
    /// from and to it "container" class (the real EF stored class)
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class InheritanceContained : Attribute
    {
   
    }
}
