TODO:

[] Order of collection form configuration is not maintained

[] Implement Return everywhere + fallback to javascript:history.back();
    [] Escape from New

[] Look for memory leaks due to not removing event handlers from components

[v] Allow for custom View elements in ListView / NodeView

[] Column ordering

[v] EditContext available in repositories

[] Propagating update 

2.0
[] Add more than one view to page (to allow for ListView + CustomBlock)
[] IEntity.Id requirement is obstrusive
[] GetRelationListViewAsync method evaluates authorization for all buttons to relatedEntity, while the processing functions will evaluate authorizaton sometimes to IEntity and sometimes to IRelatedEntity.
[] Hide sections of the tree when user is unauthorized
[] Reordering

x.x
[] All config checked during startup
    [] Check Repository compatibility with Entity
    []
[] Unit tests
