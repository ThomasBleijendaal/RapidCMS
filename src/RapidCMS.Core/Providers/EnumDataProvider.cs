using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Providers
{
    public class EnumDataProvider<TEnum> : IDataCollection
        where TEnum : Enum
    {
        public event EventHandler? OnDataChange;

        public Task<IEnumerable<IElement>> GetAvailableElementsAsync()
        {
            var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

            var list = new List<IElement>();

            foreach (var value in values)
            {
                var attribute = value.GetCustomAttribute<DisplayAttribute>();

                if (attribute != null)
                {
                    list.Add(new Element
                    {
                        Id = value,
                        Labels = new[] { attribute.Name ?? string.Empty }
                    });
                }
                else
                {
                    list.Add(new Element
                    {
                        Id = value,
                        Labels = new[] { value.ToString() }
                    });
                }
            }

            return Task.FromResult(list.AsEnumerable());
        }

        public Task SetEntityAsync(FormEditContext editContext, IParent? parent)
        {
            return Task.CompletedTask;
        }
    }
}
