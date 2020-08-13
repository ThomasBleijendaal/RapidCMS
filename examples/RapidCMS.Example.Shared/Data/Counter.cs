using System;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Example.Shared.Data
{
    public class Counter : IEntity, ICloneable
    {
        public string? Id { get; set; }

        [Display(Name = "Current count", ShortName = "Current count")]
        public int CurrentCount { get; set; }

        public object Clone()
        {
            return new Counter()
            {
                CurrentCount = CurrentCount,
                Id = Id
            };
        }
    }
}
