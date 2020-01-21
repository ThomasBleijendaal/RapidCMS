TODO:

[] BUG: Nodes always open fully when displayed (when sub collections have no visible entities)

2.0
[v] Reordering
[v] Return in sub collection entity is weird -> implement complete state breadcrumb (with search + data views)
    [v] Do not make New in list editor something new, since it is very temporary and weird to redirect to
[v] Paginate tree
[v] Do not disable buttons when form is invalid (this causes issues to trigger revalidation)
[] ListEditor + NodeEditor do not change upon Repository update (should ask for refresh)
[v] Basic file uploader
    [v] Support for configurable preview component
[v] Add specialty collection via AddSubCollectionList / AddRelatedCollectionList instead of weak alias binding
    [v] Remove ListType and EmptyVariantColumnVisibility from SetListEditor overloads and put it in config action
[] Bring back DataView<> and builder 
[v] All config checked during startup
    [v] Check Repository compatibility with Entity
[] Unit test Dispatchers
[] Update page url when navigating

2.1
[] Add more than one view to page (to allow for ListView + CustomBlock)
    - do this via allowing any subview as element to a page
    - test with repo less collections which just display as single page (call them Page and not Collection)
    - have repos and collection more combined (since Pages do thing without Repos)
[] Hide sections of the tree when user is unauthorized
[] Be able to disable buttons (and validate state in Dispatcher)

uncertain
[] More unit tests
[] Propagating update to subcollections 
    - CON: requires total rework of UI and total refactor of how list are contained inside nodes
[] Follow user down the tree + Always display where the user is 
    - CON: is this better UX? this will be changing the tree everything the user does something
[] Find solution for stray validation messages for Table / Block ListEditor
    - CON: there is no room for it in a table
    - PRO: there is room for it in a block
[] Redirect user to different edit pages / branches
    [] Alias sub collection pages and have edit / view buttons direct to them (multiple node editors for 1 collection)
    [] Redirect user to edit page of other collection
    [] Allow New to go to dedicated NodeEditor from ListEditor
