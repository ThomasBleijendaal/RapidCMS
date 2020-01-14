using System.Threading.Tasks;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Abstractions.Interactions
{
    internal interface IButtonInteraction
    {
        Task<CrudType> ValidateButtonInteractionAsync(IEditorButtonInteractionRequestModel request);
        Task CompleteButtonInteractionAsync(IEditorButtonInteractionRequestModel request);

        Task<CrudType> ValidateButtonInteractionAsync(IEditorInListInteractionRequestModel request);

        Task<(CrudType crudType, EntityVariantSetup? entityVariant)> ValidateButtonInteractionAsync(IListButtonInteractionRequestModel request);
        Task CompleteButtonInteractionAsync(IListButtonInteractionRequestModel request);
    }
}
