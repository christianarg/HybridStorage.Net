using HybridStorage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HybridPerformanceTests.Hybrid
{
    public class HybridContent
    {
        [Key, StringLength(30)]
        public string Id { get; set; }

        public string FieldsStorage { get; set; }

        public string Language { get; set; }

        [StoredModel(nameof(FieldsStorage))]
        public List<HybridField> Fields { get; set; }

        public HybridContent()
        {
            this.Fields = new List<HybridField>();
        }
    }

    public class HybridField
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }

    public enum HybridVersionType
    {
        Public,
        Pwc
    }
}
