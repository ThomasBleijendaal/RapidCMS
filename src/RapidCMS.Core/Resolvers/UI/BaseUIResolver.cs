using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        protected readonly IHttpContextAccessor _httpContextAccessor;

        protected BaseUIResolver(
            IDataProviderResolver dataProviderResolver,
            IButtonActionHandlerResolver buttonActionHandlerResolver,
            IAuthService authService,
            IHttpContextAccessor httpContextAccessor)
        {
            _dataProviderResolver = dataProviderResolver;
            _buttonActionHandlerResolver = buttonActionHandlerResolver;
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
        }

        protected async Task<List<ButtonUI>> GetButtonsAsync(IEnumerable<IButtonSetup> buttons, EditContext editContext)
        {
            return await buttons
                .GetAllButtons()
                .SelectNotNullAsync(async button =>
                {
                    var handler = _buttonActionHandlerResolver.GetButtonActionHandler(button);
                    if (handler.IsCompatible(button, editContext) && 
                        await _authService.IsUserAuthorizedAsync(editContext, button).ConfigureAwait(false))
                    {
                        return new ButtonUI(handler, button, editContext);
                    }
                    else
                    {
                        return default;
                    }
                })
                .ToListAsync().ConfigureAwait(false);
        }

        protected internal async Task<SectionUI> GetSectionUIAsync(PaneSetup pane, EditContext editContext)
        {
            var fields = pane.Fields.Select(field =>
            {
                var dataProvider = _dataProviderResolver.GetDataProvider(field);
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
                Buttons = await GetButtonsAsync(pane.Buttons, editContext).ConfigureAwait(false),
                CustomType = pane.CustomType,
                Label = pane.Label,

                Elements = fields
                    .Union(subCollections)
                    .Union(relatedCollections)
                    .OrderBy(x => x.index)
                    .ToList(x => x.element)
            };
        }

        protected FieldUI GetField(FieldSetup field, DataProvider? dataProvider)
        {
            return field switch
            {
                CustomExpressionFieldSetup x => new CustomExpressionFieldUI(x),
                ExpressionFieldSetup x => new ExpressionFieldUI(x),

                CustomPropertyFieldSetup x => new CustomPropertyFieldUI(x, dataProvider),
                PropertyFieldSetup x => new PropertyFieldUI(x, dataProvider),

                _ => default!
            };
        }
    }
}
