using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Navigation;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.EqualityComparers;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Models.UI;
using RapidCMS.Core.Navigation;

namespace RapidCMS.Core.Resolvers.UI
{
    internal class ListUIResolver : BaseUIResolver, IListUIResolver
    {
        private readonly ListSetup _list;
        private readonly IDataViewResolver _dataViewResolver;
        private readonly Dictionary<Type, IEnumerable<FieldUI>> _fieldsPerType = new Dictionary<Type, IEnumerable<FieldUI>>();

        private readonly FieldUIEqualityComparer _equalityComparer = new FieldUIEqualityComparer();

        public ListUIResolver(
            ListSetup list,
            IDataProviderResolver dataProviderService,
            IDataViewResolver dataViewResolver,
            IButtonActionHandlerResolver buttonActionHandlerResolver,
            INavigationStateProvider navigationStateProvider,
            IAuthService authService) : base(dataProviderService, buttonActionHandlerResolver, authService, navigationStateProvider)
        {
            _list = list;
            _dataViewResolver = dataViewResolver;

            _list.Panes?.ForEach(pane =>
            {
                if (!_fieldsPerType.ContainsKey(pane.VariantType) && pane.Fields != null)
                {
                    _fieldsPerType.Add(pane.VariantType, pane.Fields.Select(x => GetField(x, default)));
                }
            });
        }

        public async Task<IEnumerable<ButtonUI>> GetButtonsForEditContextAsync(FormEditContext editContext)
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
                CommonFields = _fieldsPerType.GetCommonValues(_equalityComparer).ToList(),
                EmptyVariantColumnVisibility = _list.EmptyVariantColumnVisibility,
                ListType = _list.ListType,
                MaxUniqueFieldsInSingleEntity = _fieldsPerType.Max(x => x.Value.Count()),
                PageSize = _list.PageSize ?? 1000,
                SearchBarVisible = _list.SearchBarVisible ?? true,
                Reorderable = _list.ReorderingAllowed ?? false,
                SectionsHaveButtons = _list.Panes.Any(x => x.Buttons.Any()),
                UniqueFields = _fieldsPerType.SelectMany(x => x.Value).Distinct(_equalityComparer).ToList()
            };
        }

        public async Task<IEnumerable<SectionUI>> GetSectionsForEditContextAsync(FormEditContext editContext, NavigationState navigationState)
        {
            var type = editContext.Entity.GetType();
            return await _list.Panes
                .Where(pane => pane.VariantType.IsSameTypeOrDerivedFrom(type))
                .ToListAsync(pane => GetSectionUIAsync(pane, editContext, navigationState));
        }

        public async Task<IEnumerable<TabUI>?> GetTabsAsync(string collectionAlias)
        {
            var commonFields = _fieldsPerType.GetCommonValues(_equalityComparer).ToList();

            var data = await _dataViewResolver.GetDataViewsAsync(collectionAlias);
            return data.ToList(x =>
            {
                SortBag? sortBag = null;

                if (x.DefaultOrderBys != null)
                {
                    var commonSortFields = commonFields.Select(f => (
                        field: f, 
                        orderBy: x.DefaultOrderBys.FirstOrDefault(d => 
                            f.OrderByExpression?.Fingerprint == d.Key.Fingerprint ||
                            (f.OrderByExpression == null && f.Property?.Fingerprint == d.Key.Fingerprint))
                        .Value));

                    var defaultSorts = commonSortFields
                        .Where(x => x.orderBy != OrderByType.Disabled)
                        .Select(x => new KeyValuePair<int, OrderByType>(x.field.Index, x.orderBy));

                    sortBag = new SortBag(defaultSorts);
                }

                return new TabUI(x.Id)
                {
                    Label = x.Label,
                    DefaultSorts = sortBag
                };
            });
        }
    }
}
