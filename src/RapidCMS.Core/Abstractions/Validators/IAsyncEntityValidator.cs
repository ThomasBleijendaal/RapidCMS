using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;

namespace RapidCMS.Core.Abstractions.Validators
{
    public interface IAsyncEntityValidator
    {
        Task<IEnumerable<ValidationResult>> ValidateAsync(IEntity entity, IRelationContainer relationContainer);
    }
}
