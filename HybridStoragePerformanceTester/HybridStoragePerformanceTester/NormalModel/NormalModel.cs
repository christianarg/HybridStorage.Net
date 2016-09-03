using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HybridPerformanceTests.NormalModel
{
    public class NormalContent
    {
        [Key, StringLength(30)]
        public string Id { get; set; }

        public string Language { get; set; }

        public List<NormalField> Fields { get; set; }

        public NormalContent()
        {
            this.Fields = new List<NormalField>();
        }
    }

    public class NormalField
    {
        [Key, StringLength(30)]
        public string Name { get; set; }

        public string Value { get; set; }
    }

    public enum NormalVersionType
    {
        Public,
        Pwc
    }
}
