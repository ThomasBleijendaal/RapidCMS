using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Models.UI;
using RapidCMS.Core.Providers;

namespace RapidCMS.Core.Resolvers.UI
{
    internal class BaseUIResolver
    {
        private readonly IDataProviderResolver _dataProviderResolver;
        private readonly IButtonActionHandlerResolver _buttonActionHandlerResolver;
        protected readonly IAuthService _authService;

        protected BaseUIResolver(
            IDataProviderResolver dataProviderResolver,
            IButtonActionHandlerResolver buttonActionHandlerResolver,
            IAuthService authService)
        {
            _dataProviderResolver = dataProviderResolver;
            _buttonActionHandlerResolver = buttonActionHandlerResolver;
            _authService = authService;
        }

        protected async Task<List<ButtonUI>> GetButtonsAsync(IEnumerable<IButtonSetup> buttons, FormEditContext editContext)
        {
            return await buttons
                .GetAllButtons()
                .SelectNotNullAsync(async button =>
                {
                    var handler = _buttonActionHandlerResolver.GetButtonActionHandler(button);
                    if (handler.IsCompatible(button, editContext) && 
                        await _authService.IsUserAuthorizedAsync(editContext, button))
                    {
                        return new ButtonUI(handler, button, editContext);
                    }
                    else
                    {
                        return default;
                    }
                })
                .ToListAsync();
        }

        protected internal async Task<SectionUI> GetSectionUIAsync(IPaneSetup pane, FormEditContext editContext)
        {
            var fields = await pane.Fields.ToListAsync(async field =>
            {
                var dataProvider = await _dataProviderResolver.GetDataProviderAsync(field);
                if (dataProvider != null)
                {
                    editContext.DataProviders.Add(dataProvider);
                }

                return (index: field.Index, element: (ElementUI)GetField(field, dataProvider));
            });

            var subCollections = pane.SubCollectionLists.Select(subCollection =>
            {
                return (index: subCollection.Index, element: (ElementUI)new SubCollectionUI(subCollection));
            });

            var relatedCollections = pane.RelatedCollectionLists.Select(relatedCollection =>
            {
                return (index: relatedCollection.Index, element: (ElementUI)new RelatedCollectionUI(relatedCollection));
            });

            return new SectionUI(pane.IsVisible)
            {
                Buttons = await GetButtonsAsync(pane.Buttons, editContext),
                CustomType = pane.CustomType,
                Label = pane.Label,

                Elements = fields
                    .Union(subCollections)
                    .Union(relatedCollections)
                    .OrderBy(x => x.index)
                    .ToList(x => x.element)
            };
        }

        protected FieldUI GetField(IFieldSetup field, FormDataProvider? dataProvider)
        {
            return field switch
            {
                CustomExpressionFieldSetup x => new CustomExpressionFieldUI(x),
                ExpressionFieldSetup x => new ExpressionFieldUI(x),

                CustomPropertyFieldSetup x => new CustomPropertyFieldUI(x, dataProvider),
                PropertyFieldSetup x => new PropertyFieldUI(x, dataProvider),

                _ => throw new InvalidOperationException($"Cannot return FieldUI for given field of type {field?.GetType()}")
            };
        }
    }
}
