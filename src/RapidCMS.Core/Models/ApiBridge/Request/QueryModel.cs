using System;
using Newtonsoft.Json;
using RapidCMS.Core.Abstractions.Data;
using static RapidCMS.Core.Models.Data.Query;

namespace RapidCMS.Core.Models.ApiBridge.Request
{
    public class QueryModel
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public string? SearchTerm { get; set; }

        public string? VariantTypeName { get; set; }

        [JsonIgnore]
        public IQuery Query
        {
            get
            {
                return Create(Take, 1 + Skip / Math.Max(1, Take), SearchTerm, 0);
            }
        }
    }
}
