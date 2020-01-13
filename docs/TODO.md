TODO:

[] Nodes always open fully when a sub collection has no children (?)

[] Find solution for stray validation messages for Table / Block ListEditor
    - summed up in top?

[] Return in sub collection entity is weird

2.0
[] Add more than one view to page (to allow for ListView + CustomBlock)
    - do this via allowing any subview as element to a page
    - test with repo less collections which just display as single page (call them Page and not Collection)
    - have repos and collection more combined (since Pages do thing without Repos)
[] GetRelationListViewAsync method evaluates authorization for all buttons to relatedEntity, while the processing functions will evaluate authorizaton sometimes to IEntity and sometimes to IRelatedEntity.
[] Hide sections of the tree when user is unauthorized
[] Reordering
[] Propagating update to subcollections
[] Follow user down the tree + Always display where the user is
[] Paginate tree
[] Alias sub collection pages and have edit / view buttons direct to them

2.0
[] All config checked during startup
    [v] Check Repository compatibility with Entity
    []
[] Unit tests
