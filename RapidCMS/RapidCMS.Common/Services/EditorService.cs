using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.EqualityComparers;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Forms;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.UI;

namespace RapidCMS.Common.Services
{
    internal class EditorService : IEditorService
    {
        private readonly Root _root;
        private readonly IDataProviderService _dataProviderService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EditorService(Root root, IDataProviderService dataProviderService, IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor)
        {
            _root = root;
            _dataProviderService = dataProviderService;
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<NodeUI> GetNodeAsync(UsageType usageType, string collectionAlias)
        {
            var collection = _root.GetCollection(collectionAlias);
            var node = usageType.HasFlag(UsageType.View) ? collection.NodeView : collection.NodeEditor;
            if (node == null)
            {
                throw new InvalidOperationException($"Failed to get UI configuration from collection {collectionAlias} for action {usageType}");
            }

            var nodeUI = new NodeUI(ButtonCallAsync, ListCallAsync);

            return Task.FromResult(nodeUI);

            async Task<List<ButtonUI>?> ButtonCallAsync(EditContext editContext)
            {
                if (node.Buttons == null)
                {
                    return null;
                }

                return await GetButtonsAsync(node.Buttons, editContext);
            }
            async Task<List<SectionUI>?> ListCallAsync(EditContext editContext)
            {
                if (node.EditorPanes == null)
                {
                    return null;
                }

                var type = editContext.Entity.GetType();

                return await node.EditorPanes
                    .Where(pane => pane.VariantType.IsSameTypeOrBaseTypeOf(type))
                    .ToListAsync(pane => GetSectionUIAsync(pane, editContext));
            }
        }

        public Task<ListUI> GetListAsync(UsageType usageType, string collectionAlias)
        {
            var collection = _root.GetCollection(collectionAlias);

            List<Button>? buttons;
            List<Pane>? panes;
            int? pageSize;
            bool? searchBarVisible;
            ListType listType;

            if (usageType == UsageType.List || usageType.HasFlag(UsageType.Add))
            {
                var listView = collection.ListView;
                if (listView == null)
                {
                    throw new InvalidOperationException($"Failed to get UI configuration from collection {collectionAlias} for action {usageType}");
                }

                panes = listView.Panes;
                buttons = listView.Buttons;
                pageSize = listView.PageSize;
                searchBarVisible = listView.SearchBarVisible;
                listType = listView.ListType;
            }
            else
            {
                var listEditor = collection.ListEditor;
                if (listEditor == null)
                {
                    throw new InvalidOperationException($"Failed to get UI configuration from collection {collectionAlias} for action {usageType}");
                }

                panes = listEditor.Panes;
                buttons = listEditor.Buttons;
                pageSize = listEditor.PageSize;
                searchBarVisible = listEditor.SearchBarVisible;
                listType = listEditor.ListType;
            }

            // TODO: this thing naively assumes only one fieldset per type
            var fieldsPerType = new Dictionary<Type, IEnumerable<FieldUI>>();
            var sectionsHaveButtons = panes.Any(x => x.Buttons.Any());

            panes?.ForEach(pane =>
            {
                if (!fieldsPerType.ContainsKey(pane.VariantType) && pane.Fields != null)
                {
                    fieldsPerType.Add(pane.VariantType, pane.Fields.Select(x => x.ToUI()));
                }
            });

            var list = new ListUI(ButtonCallAsync, ListCallAsync, TabCallAsync)
            {
                ListType = listType,
                SectionsHaveButtons = sectionsHaveButtons,
                PageSize = pageSize ?? 1000, // TODO: config setting?
                SearchBarVisible = searchBarVisible ?? true
            };

            if (fieldsPerType.Any())
            {
                list.MaxUniqueFieldsInSingleEntity = fieldsPerType.Max(x => x.Value.Count());
                list.UniqueFields = fieldsPerType.SelectMany(x => x.Value).Distinct(new FieldUIEqualityComparer()).ToList();
                list.CommonFields = fieldsPerType.GetCommonValues(new FieldUIEqualityComparer()).ToList();
            }

            return Task.FromResult(list);

            async Task<List<ButtonUI>?> ButtonCallAsync(EditContext editContext)
            {
                if (buttons == null)
                {
                    return null;
                }

                return await GetButtonsAsync(buttons, editContext);
            }

            async Task<List<SectionUI>?> ListCallAsync(EditContext editContext)
            {
                var type = editContext.Entity.GetType();

                return await panes
                    .Where(pane => pane.VariantType.IsSameTypeOrDerivedFrom(type))
                    .ToListAsync(pane => GetSectionUIAsync(pane, editContext));
            }

            async Task<List<TabUI>?> TabCallAsync(EditContext editContext)
            {
                var data = await collection.GetDataViewsAsync(editContext);

                return data.ToList(x => new TabUI { Id = x.Id, Label = x.Label });
            }
        }

        private async Task<SectionUI> GetSectionUIAsync(Pane pane, EditContext editContext)
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

            return new SectionUI(pane.CustomAlias, pane.Label, pane.IsVisible)
            {
                Buttons = await GetButtonsAsync(pane.Buttons, editContext),

                Elements = fields
                    .Union(subCollections)
                    .Union(relatedCollections)
                    .OrderBy(x => x.index)
                    .ToList(x => x.element)
            };
        }

        private async Task<List<ButtonUI>> GetButtonsAsync(IEnumerable<Button> buttons, EditContext editContext)
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
    }
}
