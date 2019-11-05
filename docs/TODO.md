TODO:

[] Order of collection form configuration is not maintained

[] Implement Return everywhere + fallback to javascript:history.back();
    [] Escape from New

[] Look for memory leaks due to not removing event handlers from components

[] Allow for custom View elements in ListView / NodeView

[] Column ordering

[] EditContext available in repositories

2.0
[] Add more than one view to page (to allow for ListView + CustomBlock)
[] IEntity.Id requirement is obstrusive
[] Extensible Dropdown (type ahead style insert of new elements + automatic update when used in list views)
[] GetRelationListViewAsync method evaluates authorization for all buttons to relatedEntity, while the processing functions will evaluate authorizaton sometimes to IEntity and sometimes to IRelatedEntity.
[] Hide sections of the tree when user is unauthorized
[] Reordering

x.x
[] All config checked during startup
    [] Check Repository compatibility with Entity
    []
[] Unit tests
