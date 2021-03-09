using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.EqualityComparers;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Models.UI;

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
            IAuthService authService) : base(dataProviderService, buttonActionHandlerResolver, authService)
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
            var commonValues = _fieldsPerType.GetCommonValues(_equalityComparer).ToList();

            return new ListUI
            {
                CommonFields = commonValues,
                EmptyVariantColumnVisibility = _list.EmptyVariantColumnVisibility,
                ListType = _list.ListType,
                MaxUniqueFieldsInSingleEntity = _fieldsPerType.Max(x => x.Value.Count()),
                PageSize = _list.PageSize ?? 1000,
                SearchBarVisible = _list.SearchBarVisible ?? true,
                Reorderable = _list.ReorderingAllowed ?? false,
                SectionsHaveButtons = _list.Panes.Any(x => x.Buttons.Any()),
                UniqueFields = _fieldsPerType.SelectMany(x => x.Value).Distinct(_equalityComparer).ToList(),
                GroupedFields = _fieldsPerType
                    .ToDictionary(x => x.Key, x => x.Value.Where())
            };
        }

        public async Task<IEnumerable<SectionUI>> GetSectionsForEditContextAsync(FormEditContext editContext)
        {
            var type = editContext.Entity.GetType();
            return await _list.Panes
                .Where(pane => pane.VariantType.IsSameTypeOrDerivedFrom(type))
                .ToListAsync(pane => GetSectionUIAsync(pane, editContext));
        }

        public async Task<IEnumerable<TabUI>?> GetTabsAsync(FormEditContext editContext)
        {
            var data = await _dataViewResolver.GetDataViewsAsync(editContext.CollectionAlias);
            return data.ToList(x => new TabUI(x.Id) { Label = x.Label });
        }
    }
}
