using HybridStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HybridStorageTests.TestModel.AdvancedTestModel
{

    public class Content
    {
        public string Id { get; set; }
        public string Language { get; set; }

        public string VersionStorage { get; set; }

        [StoredModel("VersionStorage")]
        public ContentVersion Version { get; set; }
    }

    public class ContentVersion
    {
        public ContentVersion()
        {
            Fields = new List<Field>();
        }

        public int Number { get; set; }
        public VersionType VersionType { get; set; }
        public List<Field> Fields { get; set; }
    }

    public class Field
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public enum VersionType
    {
        Public,
        Pwc
    }
}
