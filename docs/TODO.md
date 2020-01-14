TODO:

[] BUG: Nodes always open fully when displayed (when sub collections have no visible entities)

[] Find solution for stray validation messages for Table / Block ListEditor
    - summed up in top?

[] Return in sub collection entity is weird -> implement complete state breadcrumb (with search + data views)

2.0
[] Redirect user to different edit pages / branches
    [] Alias sub collection pages and have edit / view buttons direct to them (multiple node editors for 1 collection)
[] Hide sections of the tree when user is unauthorized
[] Reordering
[] Propagating update to subcollections
[] Follow user down the tree + Always display where the user is
[] Paginate tree
[v] Do not disable buttons when form is invalid (this causes issues to trigger revalidation)

2.0
[] All config checked during startup
    [v] Check Repository compatibility with Entity
    []
[] Unit tests

2.1
[] Add more than one view to page (to allow for ListView + CustomBlock)
    - do this via allowing any subview as element to a page
    - test with repo less collections which just display as single page (call them Page and not Collection)
    - have repos and collection more combined (since Pages do thing without Repos)
