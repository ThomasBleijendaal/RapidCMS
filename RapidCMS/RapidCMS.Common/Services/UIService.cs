using System;
using System.Collections.Generic;
using System.Linq;
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
    internal class UIService : IUIService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthorizationService _authorizationService;

        public UIService(IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor, IAuthorizationService authorizationService)
        {
            _serviceProvider = serviceProvider;
            _httpContextAccessor = httpContextAccessor;
            _authorizationService = authorizationService;
        }

        public async Task<NodeUI> GenerateNodeUIAsync(ViewContext viewContext, EditContext editContext, Node nodeEditor)
        {
            return new NodeUI(editContext)
            {
                Buttons = nodeEditor.Buttons == null
                    ? null
                    : await nodeEditor.Buttons
                        .GetAllButtons()
                        .Where(button => button.IsCompatibleWithView(viewContext))
                        .WhereAsync(async button =>
                        {
                            var authorizationChallenge = await _authorizationService.AuthorizeAsync(
                                _httpContextAccessor.HttpContext.User,
                                viewContext.RepresentativeEntity,
                                Operations.GetOperationForCrudType(button.GetCrudType()));

                            return authorizationChallenge.Succeeded;
                        })
                        .ToListAsync(button => button.ToUI()),

                Sections = nodeEditor.EditorPanes
                    .Where(pane => pane.VariantType.IsSameTypeOrBaseTypeOf(viewContext.EntityVariant.Type))
                    .ToList(pane =>
                    {
                        var fields = pane.Fields.Select(field =>
                        {
                            return (field.Index, element: (Element)field.ToUI(_serviceProvider));
                        });

                        var subCollections = pane.SubCollectionLists.Select(subCollection =>
                        {
                            return (subCollection.Index, element: (Element)subCollection.ToUI());
                        });

                        return new SectionUI
                        {
                            CustomAlias = pane.CustomAlias,
                            Label = pane.Label,

                            Elements = fields.Union(subCollections)
                                .OrderBy(x => x.Index)
                                .ToList(x => x.element)
                        };
                    })
            };
        }

        public async Task<ListUI> GenerateListUIAsync(ViewContext listViewContext, EditContext rootEditContext, IEnumerable<EditContext> editContexts, Func<EditContext, ViewContext> entityViewContext, ListView listView)
        {
            var fieldsPerType = new Dictionary<Type, IEnumerable<FieldUI>>();
            var sectionsHaveButtons = false;

            var list = new ListUI(rootEditContext, editContexts)
            {
                ListType = ListType.TableView,

                Buttons = listView.Buttons == null
                    ? null
                    : await listView.Buttons
                        .GetAllButtons()
                        .Where(button => button.IsCompatibleWithView(listViewContext))
                        .WhereAsync(async button =>
                        {
                            var authorizationChallenge = await _authorizationService.AuthorizeAsync(
                                _httpContextAccessor.HttpContext.User,
                                listViewContext.RepresentativeEntity,
                                Operations.GetOperationForCrudType(button.GetCrudType()));

                            return authorizationChallenge.Succeeded;
                        })
                        .ToListAsync(button => button.ToUI()),

                SectionsForEntity = listView.ViewPane == null
                    ? null
                    : await editContexts.ToDictionaryAsync(
                        editContext => editContext.Entity.Id,
                        async editContext =>
                        {
                            var viewContext = entityViewContext(editContext);
                            var type = editContext.Entity.GetType();

                            // TODO: view pane not yet capable of entity variants
                            var section = new SectionUI
                            {
                                // TODO: custom?
                                // CustomAlias

                                Buttons = await listView.ViewPane.Buttons
                                        .GetAllButtons()
                                        .Where(button => button.IsCompatibleWithView(viewContext))
                                        .WhereAsync(async button =>
                                        {
                                            var authorizationChallenge = await _authorizationService.AuthorizeAsync(
                                                _httpContextAccessor.HttpContext.User,
                                                editContext.Entity,
                                                Operations.GetOperationForCrudType(button.GetCrudType()));

                                            return authorizationChallenge.Succeeded;
                                        })
                                        .ToListAsync(button => button.ToUI()),

                                Elements = listView.ViewPane.Fields.ToList(field => (Element)field.ToUI(_serviceProvider))
                            };
                            
                            if (!fieldsPerType.ContainsKey(type) && section.Elements != null)
                            {
                                fieldsPerType.Add(type, section.Elements.Where(x => x is FieldUI).ToList(x => (FieldUI)x));
                            }

                            sectionsHaveButtons = sectionsHaveButtons || (section.Buttons?.Any() ?? false);

                            return new List<SectionUI> { section };
                        })
            };

            list.SectionsHaveButtons = sectionsHaveButtons;

            if (fieldsPerType.Any())
            {
                list.MaxUniqueFieldsInSingleEntity = fieldsPerType.Max(x => x.Value.Count());
                list.UniqueFields = fieldsPerType.SelectMany(x => x.Value).Distinct(new FieldUIEqualityComparer()).ToList();
                list.CommonFields = fieldsPerType.GetCommonValues(new FieldUIEqualityComparer()).ToList();
            }

            return list;
        }

        public async Task<ListUI> GenerateListUIAsync(ViewContext listViewContext, EditContext rootEditContext, IEnumerable<EditContext> editContexts, Func<EditContext, ViewContext> entityViewContext, ListEditor listEditor)
        {
            var fieldsPerType = new Dictionary<Type, IEnumerable<FieldUI>>();
            var sectionsHaveButtons = false;

            var list = new ListUI(rootEditContext, editContexts)
            {
                ListType = listEditor.ListEditorType == ListEditorType.Table
                    ? ListType.TableEditor
                    : ListType.BlockEditor,

                EmptyVariantColumnVisibility = listEditor.EmptyVariantColumnVisibility,

                Buttons = listEditor.Buttons == null
                    ? null
                    : await listEditor.Buttons
                        .GetAllButtons()
                        .Where(button => button.IsCompatibleWithView(listViewContext))
                        .WhereAsync(async button =>
                        {
                            var authorizationChallenge = await _authorizationService.AuthorizeAsync(
                                _httpContextAccessor.HttpContext.User,
                                listViewContext.RepresentativeEntity,
                                Operations.GetOperationForCrudType(button.GetCrudType()));

                            return authorizationChallenge.Succeeded;
                        })
                        .ToListAsync(button => button.ToUI()),

                SectionsForEntity = listEditor.EditorPanes == null
                    ? null
                    : await editContexts.ToDictionaryAsync(
                        editContext => editContext.Entity.Id,
                        async editContext =>
                        {
                            var viewContext = entityViewContext(editContext);
                            var type = editContext.Entity.GetType();

                            return await listEditor.EditorPanes
                                .Where(pane => pane.VariantType.IsSameTypeOrDerivedFrom(type))
                                .ToListAsync(async pane =>
                                {
                                    var section = new SectionUI
                                    {
                                        CustomAlias = pane.CustomAlias,

                                        Buttons = await pane.Buttons
                                            .GetAllButtons()
                                            .Where(button => button.IsCompatibleWithView(viewContext))
                                            .WhereAsync(async button =>
                                            {
                                                var authorizationChallenge = await _authorizationService.AuthorizeAsync(
                                                    _httpContextAccessor.HttpContext.User,
                                                    editContext.Entity,
                                                    Operations.GetOperationForCrudType(button.GetCrudType()));

                                                return authorizationChallenge.Succeeded;
                                            })
                                            .ToListAsync(button => button.ToUI()),

                                        Elements = pane.Fields.ToList(field => (Element)field.ToUI(_serviceProvider))
                                    };

                                    if (!fieldsPerType.ContainsKey(type) && section.Elements != null)
                                    {
                                        fieldsPerType.Add(type, section.Elements.Where(x => x is FieldUI).ToList(x => (FieldUI)x));
                                    }

                                    sectionsHaveButtons = sectionsHaveButtons || (section.Buttons?.Any() ?? false);

                                    return section;
                                });
                        })
            };

            list.SectionsHaveButtons = sectionsHaveButtons;

            if (fieldsPerType.Any())
            {
                list.MaxUniqueFieldsInSingleEntity = fieldsPerType.Max(x => x.Value.Count());
                list.UniqueFields = fieldsPerType.SelectMany(x => x.Value).Distinct(new FieldUIEqualityComparer()).ToList();
                list.CommonFields = fieldsPerType.GetCommonValues(new FieldUIEqualityComparer()).ToList();
            }

            return list;
        }
    }
}
