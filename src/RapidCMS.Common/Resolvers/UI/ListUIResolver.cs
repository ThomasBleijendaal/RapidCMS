using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using RapidCMS.Common.EqualityComparers;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Forms;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.UI;
using RapidCMS.Common.Services;

namespace RapidCMS.Common.Resolvers.UI
{
    internal class ListUIResolver : BaseUIResolver, IListUIResolver
    {
        private readonly List _list;
        private readonly Collection _collection;
        private readonly Dictionary<Type, IEnumerable<FieldUI>> _fieldsPerType = new Dictionary<Type, IEnumerable<FieldUI>>();

        public ListUIResolver(
            List list,
            Collection collection,
            IDataProviderService dataProviderService,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor) : base(dataProviderService, authorizationService, httpContextAccessor)
        {
            _list = list;
            _collection = collection;

            _list.Panes?.ForEach(pane =>
            {
                if (!_fieldsPerType.ContainsKey(pane.VariantType) && pane.Fields != null)
                {
                    _fieldsPerType.Add(pane.VariantType, pane.Fields.Select(x => x.ToUI()));
                }
            });
        }

        public async Task<IEnumerable<ButtonUI>> GetButtonsForEditContextAsync(EditContext editContext)
        {
            if (_list.Buttons == null)
            {
                return Enumerable.Empty<ButtonUI>();
            }

            return await GetButtonsAsync(_list.Buttons, editContext);
        }

        public ListUI GetListDetails()
        {
            return new ListUI
            {
                CommonFields = _fieldsPerType.GetCommonValues(new FieldUIEqualityComparer()).ToList(),
                EmptyVariantColumnVisibility = _list.EmptyVariantColumnVisibility,
                ListType = _list.ListType,
                MaxUniqueFieldsInSingleEntity = _fieldsPerType.Max(x => x.Value.Count()),
                PageSize = _list.PageSize ?? 1000,
                SearchBarVisible = _list.SearchBarVisible ?? true,
                SectionsHaveButtons = _list.Panes.Any(x => x.Buttons.Any()),
                UniqueFields = _fieldsPerType.SelectMany(x => x.Value).Distinct(new FieldUIEqualityComparer()).ToList()
            };
        }

        public async Task<IEnumerable<SectionUI>> GetSectionsForEditContextAsync(EditContext editContext)
        {
            var type = editContext.Entity.GetType();

            return await _list.Panes
                .Where(pane => pane.VariantType.IsSameTypeOrDerivedFrom(type))
                .ToListAsync(pane => GetSectionUIAsync(pane, editContext));
        }

        public async Task<IEnumerable<TabUI>?> GetTabsAsync(EditContext editContext)
        {
            var data = await _collection.GetDataViewsAsync(editContext);

            return data.ToList(x => new TabUI { Id = x.Id, Label = x.Label });
        }
    }
}
