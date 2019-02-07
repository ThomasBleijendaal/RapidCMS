using System;
using System.Collections.Generic;
using System.Text;

namespace RapidCMS.Common.Models.DTOs
{
    public class CollectionTreeDTO
    {
        public string Name { get; set; }
        public List<CollectionTreeEntity> Entities { get; set; }
    }

    public class CollectionTreeEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
