TODO:

[] Implement Return everywhere + fallback to javascript:history.back();
    [] Escape from New

[] Look for memory leaks due to not removing event handlers from components

[] Column ordering

[] Nodes always open fully when a sub collection has no children (?)

[] Deleting node in nested ListEditors gives error due to unfound List

2.0
[] Add more than one view to page (to allow for ListView + CustomBlock)
[] IEntity.Id requirement is obstrusive
[] GetRelationListViewAsync method evaluates authorization for all buttons to relatedEntity, while the processing functions will evaluate authorizaton sometimes to IEntity and sometimes to IRelatedEntity.
[] Hide sections of the tree when user is unauthorized
[] Reordering
[] Propagating update to subcollections
[] Follow user down the tree
[] Paginate tree

x.x
[] All config checked during startup
    [] Check Repository compatibility with Entity
    []
[] Unit tests
