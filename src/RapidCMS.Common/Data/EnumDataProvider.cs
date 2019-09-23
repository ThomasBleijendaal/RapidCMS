using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Models.DTOs;

namespace RapidCMS.Common.Data
{
    public class EnumDataProvider<TEnum> : IDataCollection
        where TEnum : Enum
    {
        public event EventHandler OnDataChange;

        public Task<IEnumerable<IElement>> GetAvailableElementsAsync()
        {
            var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

            var list = new List<IElement>();

            foreach (var value in values)
            {
                var attribute = value.GetCustomAttribute<DisplayAttribute>();

                if (attribute != null)
                {
                    list.Add(new ElementDTO
                    {
                        Id = value,
                        Labels = new[] { attribute.Name }
                    });
                }
                else
                {
                    list.Add(new ElementDTO
                    {
                        Id = value,
                        Labels = new[] { value.ToString() }
                    });
                }
            }

            return Task.FromResult(list.AsEnumerable());
        }

        public Task SetEntityAsync(IEntity entity)
        {
            return Task.CompletedTask;
        }
    }
}
