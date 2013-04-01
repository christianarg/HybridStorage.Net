using HybridStorage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HybridStorageTests.TestModel.SimpleTestModel
{
    public class SimpleContent
    {
        public string Id { get; set; }
        public string Language { get; set; }

        public string VersionStorage { get; set; }
        [NotMapped] // We need to tell to EF to ignore this property
        [StoredModel("VersionStorage")]
        public SimpleContentVersion Version { get; set; }
    }

    public class SimpleContentVersion
    {
        public int Number { get; set; }
    }
}