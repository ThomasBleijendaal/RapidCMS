using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Abstractions.Interactions
{
    internal interface IButtonInteraction
    {
        Task<CrudType> ValidateButtonInteractionAsync(IEditorButtonInteractionRequestModel request);
        Task CompleteButtonInteractionAsync(IEditorButtonInteractionRequestModel request);

        Task<CrudType> ValidateButtonInteractionAsync(IEditorInListInteractionRequestModel request);

        Task<(CrudType crudType, IEntityVariantSetup? entityVariant)> ValidateButtonInteractionAsync(IListButtonInteractionRequestModel request);
        Task CompleteButtonInteractionAsync(IListButtonInteractionRequestModel request);
    }
}
