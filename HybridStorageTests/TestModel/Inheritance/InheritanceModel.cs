using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using AutoMapper;
using HybridStorage;

namespace HybridStorageTests.TestModel.Inheritance
{
    public class ContentContainer : HybridEntityContainerBase<Content>
    {
        [Key]
        [StringLength(20)]
        public string Id { get; set; }

        public string Language { get; set; }

        public ContentContainer() { }
        public ContentContainer(Content content) : base(content) { }
        
    }

    public abstract class Content
    {
        public string Language { get; set; }
        protected Content(string id)
        {
            this.Id = id;
        }
        public string Id { get; set; }
    }

    public class InfoContent : Content
    {
        public InfoContent(string id) : base(id)
        {
        }

        public List<Field> Fields { get; set; }
    }

    public class Field
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class ResourceContent : Content
    {
        public ResourceContent(string id) : base(id)
        {
        }

        public string Extension { get; set; }
    }

}
