using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using RapidCMS.Common.Authorization;
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
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EditorService(
            Root root,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _root = root;
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

                return await node.Buttons
                    .GetAllButtons()
                    .Where(button => button.IsCompatibleWithForm(editContext))
                    .WhereAsync(async button =>
                    {
                        var authorizationChallenge = await _authorizationService.AuthorizeAsync(
                            _httpContextAccessor.HttpContext.User,
                            editContext.Entity,
                            Operations.GetOperationForCrudType(button.GetCrudType()));

                        return authorizationChallenge.Succeeded;
                    })
                    .ToListAsync(button => button.ToUI());
            }
            async Task<List<SectionUI>?> ListCallAsync(EditContext editContext)
            {
                if (node.EditorPanes == null)
                {
                    return null;
                }

                var type = editContext.Entity.GetType();

                // TODO: weird
                var dataContext = new DataContext(node.EditorPanes, editContext);
                editContext.DataContext = dataContext;

                return EnumerableExtensions.ToList<Pane, SectionUI>(Enumerable.Where<Pane>(node.EditorPanes,
                    pane => pane.VariantType.IsSameTypeOrBaseTypeOf(type)),
                    pane =>
                    {
                        var fields = Enumerable.Select<Field, (int Index, ElementUI element)>(pane.Fields, field =>
                        {
                            return (field.Index, element: (ElementUI)field.ToUI(editContext, dataContext));
                        });

                        var subCollections = Enumerable.Select<SubCollectionList, (int Index, ElementUI element)>(pane.SubCollectionLists, subCollection =>
                        {
                            return (subCollection.Index, element: (ElementUI)subCollection.ToUI());
                        });

                        var relatedCollections = Enumerable.Select<RelatedCollectionList, (int Index, ElementUI element)>(pane.RelatedCollectionLists, relatedCollection =>
                        {
                            return (relatedCollection.Index, element: (ElementUI)relatedCollection.ToUI());
                        });

                        return new SectionUI
                        {
                            CustomAlias = pane.CustomAlias,
                            Label = pane.Label,

                            Elements = EnumerableExtensions.ToList<(int Index, ElementUI element), ElementUI>(fields
                                .Union<(int Index, ElementUI element)>(subCollections)
                                .Union<(int Index, ElementUI element)>(relatedCollections)
                                .OrderBy<(int Index, ElementUI element), int>(x => x.Index), x => x.element)
                        };
                    });
            }

        }

        public Task<ListUI> GetListAsync(UsageType usageType, string collectionAlias)
        {
            var collection = _root.GetCollection(collectionAlias);

            List<Button>? buttons;
            List<Pane>? panes;
            int? pageSize;

            // TODO: merge IF
            if (usageType == UsageType.List || usageType.HasFlag(UsageType.Add))
            {
                var listView = collection.ListView;
                if (listView == null)
                {
                    throw new InvalidOperationException($"Failed to get UI configuration from collection {collectionAlias} for action {usageType}");
                }

                panes = listView.ViewPanes;
                buttons = listView.Buttons;
                pageSize = listView.PageSize;
            }
            else
            {
                var editView = collection.ListEditor;
                if (editView == null)
                {
                    throw new InvalidOperationException($"Failed to get UI configuration from collection {collectionAlias} for action {usageType}");
                }

                panes = editView.EditorPanes;
                buttons = editView.Buttons;
                pageSize = editView.PageSize;
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

            var list = new ListUI(ButtonCallAsync, ListCallAsync)
            {
                ListType = ListType.TableView,
                SectionsHaveButtons = sectionsHaveButtons,
                PageSize = pageSize ?? 1000 // TODO: config setting?
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

                return await buttons
                    .GetAllButtons()
                    .Where(button => button.IsCompatibleWithForm(editContext))
                    .WhereAsync(async button =>
                    {
                        //var authorizationChallenge = await _authorizationService.AuthorizeAsync(
                        //    _httpContextAccessor.HttpContext.User,
                        //    editContext.Entity,
                        //    Operations.GetOperationForCrudType(button.GetCrudType()));

                        //return authorizationChallenge.Succeeded;

                        return true;
                    })
                    .ToListAsync(button => button.ToUI());
            }

            async Task<List<SectionUI>?> ListCallAsync(EditContext editContext)
            {
                var type = editContext.Entity.GetType();

                // TODO: weird
                var dataContext = new DataContext(panes, editContext);
                editContext.DataContext = dataContext;

                return await panes
                    .Where(pane => pane.VariantType.IsSameTypeOrDerivedFrom(type))
                    .ToListAsync(async pane =>
                    {
                        // TODO: view pane not yet capable of entity variants
                        var section = new SectionUI
                        {
                            CustomAlias = pane.CustomAlias,

                            Buttons = await pane.Buttons
                            .GetAllButtons()
                            .Where(button => button.IsCompatibleWithForm(editContext))
                            .WhereAsync(async button =>
                            {
                                //var authorizationChallenge = await _authorizationService.AuthorizeAsync(
                                //    _httpContextAccessor.HttpContext.User,
                                //    editContext.Entity,
                                //    Operations.GetOperationForCrudType(button.GetCrudType()));

                                //return authorizationChallenge.Succeeded;

                                return true;
                            })
                            .ToListAsync(button => button.ToUI()),

                            Elements = pane.Fields.ToList(field => (ElementUI)field.ToUI(editContext, dataContext))
                        };

                        return section;
                    });
            }
        }
    }
}
