using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using RapidCMS.Common.Authorization;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.UI;
using RapidCMS.Common.Validation;

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
                Buttons = await nodeEditor.Buttons
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
            return new ListUI(rootEditContext, editContexts)
            {
                ListType = ListType.TableView,

                Buttons = listView.Buttons == null
                    ? new List<ButtonUI>()
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

                SectionForEntityAsync = async (entity) =>
                {
                    if (listView.ViewPane == null)
                    {
                        return null;
                    }
                    else
                    {
                        var viewContext = entityViewContext(entity);

                        return new SectionUI
                        {
                            // TODO: custom?
                            // CustomAlias = (listView.ViewPane is CustomEditorPane customEditorPane) ? customEditorPane.Alias : null,

                            Buttons = await listView.ViewPane.Buttons
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

                            Elements = listView.ViewPane.Fields.ToList(field => (Element)field.ToUI(_serviceProvider))
                        };
                    }
                }
            };
        }

        public async Task<ListUI> GenerateListUIAsync(ViewContext listViewContext, EditContext rootEditContext, IEnumerable<EditContext> editContexts, Func<EditContext, ViewContext> entityViewContext, ListEditor listEditor)
        {
            return new ListUI(rootEditContext, editContexts)
            {
                ListType = listEditor.ListEditorType == ListEditorType.Table
                    ? ListType.TableEditor
                    : ListType.BlockEditor,

                Buttons = await listEditor.Buttons
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

                SectionForEntityAsync = async (subject) =>
                {
                    var pane = listEditor.EditorPanes.FirstOrDefault(pane => pane.VariantType.IsSameTypeOrDerivedFrom(subject.Entity.GetType()));

                    if (pane == null)
                    {
                        return null;
                    }

                    var viewContext = entityViewContext(subject);

                    return new SectionUI
                    {
                        CustomAlias = pane.CustomAlias,

                        Buttons = await pane.Buttons
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

                        Elements = pane.Fields.ToList(field => (Element)field.ToUI(_serviceProvider))
                    };
                }
            };
        }
    }
}
