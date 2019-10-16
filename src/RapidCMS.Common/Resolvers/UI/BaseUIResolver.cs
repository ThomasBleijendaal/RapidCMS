using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Forms;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.UI;
using RapidCMS.Common.Services;

namespace RapidCMS.Common.Resolvers.UI
{
    internal class BaseUIResolver
    {
        protected readonly IDataProviderService _dataProviderService;
        protected readonly IAuthorizationService _authorizationService;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        protected BaseUIResolver(
            IDataProviderService dataProviderService,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _dataProviderService = dataProviderService;
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        protected async Task<List<ButtonUI>> GetButtonsAsync(IEnumerable<Button> buttons, EditContext editContext)
        {
            return await buttons
                .GetAllButtons()
                .Where(button => button.IsCompatible(editContext))
                .WhereAsync(async button =>
                {
                    var authorizationChallenge = await _authorizationService.AuthorizeAsync(
                        _httpContextAccessor.HttpContext.User,
                        editContext.Entity,
                        button.GetOperation(editContext));

                    return authorizationChallenge.Succeeded;
                })
                .ToListAsync(button => button.ToUI(editContext));
        }

        protected internal async Task<SectionUI> GetSectionUIAsync(Pane pane, EditContext editContext)
        {
            var fields = pane.Fields.Select(field =>
            {
                var dataProvider = _dataProviderService.GetDataProvider(field);
                if (dataProvider != null)
                {
                    editContext.DataProviders.Add(dataProvider);
                }

                return (index: field.Index, element: (ElementUI)field.ToUI(dataProvider));
            });

            var subCollections = pane.SubCollectionLists.Select(subCollection =>
            {
                return (index: subCollection.Index, element: (ElementUI)subCollection.ToUI());
            });

            var relatedCollections = pane.RelatedCollectionLists.Select(relatedCollection =>
            {
                return (index: relatedCollection.Index, element: (ElementUI)relatedCollection.ToUI());
            });

            return new SectionUI(pane.CustomType, pane.Label, pane.IsVisible)
            {
                Buttons = await GetButtonsAsync(pane.Buttons, editContext),

                Elements = fields
                    .Union(subCollections)
                    .Union(relatedCollections)
                    .OrderBy(x => x.index)
                    .ToList(x => x.element)
            };
        }
    }
}
