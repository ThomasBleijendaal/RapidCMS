using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Abstractions.UI;

public interface IBaseUIElement
{
    IEntity Entity { get; }
    EntityState EntityState { get; }
    Func<object, EntityState, Task<object?>>? Configuration { get; }
}
