TODO:

1.0

[] Documentation + Examples

[] Auto-update DataCollection editors when collection indicates update

[] bug: ListEditor ignores RootVariant section when using multiple EntityVariants
[] bug: Only update affected EditContext in ListEditor to prevent loss of edits in other EditContexts
[] bug: ListEditor with a lot of Dropdowns becomes very slow when clicking New (cache the fetch call)
[] issue: Applying invalid change to Entity will still update entity, and if attached, will result in db change when SaveChanges()
[] issue: Tabbar resets view when New / Add button is clicked

[] Move relation validation to Attribute
    - Require a property to be given in AddField for relations
    - Get validation attribute of that property
    - No more validation function in startup

[v] Date editor

1.1

[v] Fix nullability in PropertyMetadata

[v] Dashboard

[] Merge 2 relation patterns into a single one (either via IRelationContainer or via IRepository methods)

[v] Pagination 
    [] consistent during session
    [] Keep edit contexts in cache

[] Order of collection form configuration is not maintained
  
[] Redirect after deletion + consistent pagination / Return to parent button action
[] Redirect after insertions + no update authorization goes to 403 page

[] Property, Field, etc naming not totally consistent -> Refactor all names and methods

[] Refresh data / relation collections upon save
    [] More events from EditContext and bind nested EditContext to root
    [] Make side bar tree react to collection actions (CRUD)
        [] Make possible to open sub collections with hidden root element
    [] OnBeforeUnload EditContext IsModified check

[] Custom Section support in Collection.razor (lists) (RowSection)
    [] Fix further

[] Remove UIEditor, and render editor directly

[] Custom modal / form based on button click

1.2

[] Implement Return everywhere + fallback to javascript:history.back();

[] Reordering
    [] New IRepository methods
        -> {unknown}
    [] Update to Blazor preview 6 to support @key on EditContext
        [] In listview: After updating entity 2 the EditContext of entity 1 is reset

[] Merge NodeViewPane and NodeEditorPaneConfig

1.3

[] Button support everywhere (no more new List<Button>)
    [] ButtonActionHandler as generic parameter to CustomButton
    [] Bind CustomButton and ActionHandler more together

[] Top button bar should be side bar with meta
[] Navigation command during pre-render (server side rendering) fails
[] Use Blazor CSS parameter features

1.4

[] Add more than one view to page (to allow for ListView + CustomBlock)

2.0
[] Repository parenId is brittle (is the parent the same entity type, or completely something different? must include parent entity type somehow)
    [] ParentId must be IEntity to support different types of nesting of repos
    [] Repository compatibility must be checked when using these nestings
    [] Buttons must be able to point to editors in other collections (view in collection-a, but edit in collection-b)
    [] Remove collection-alias and move to some other system
[] Different set of collections for different entity variant (probably not needed)
[] IEntity.Id requirement is obstrusive
[v] Editor visibility based upon predicates of current IEntity or evaluation by some object
[] Sub collection buttons connected to collection to prevent numerous buttons on each sub collection row (like Update All)
[] Extensible Dropdown (type ahead style insert of new elements + automatic update when used in list views)
[] Put library statics in library (css / icons / js)
[] Investigate if GenericXConfig -> XConfig -> X can be reduces to GenericXConfig -> X (No more FieldConfig<> -> FieldConfig -> Field, but FieldConfig<> -> Field)
[] GetRelationListViewAsync method evaluates authorization for all buttons to relatedEntity, while the processing functions will evaluate authorizaton sometimes to IEntity and sometimes to IRelatedEntity.
[] Hide sections of the tree when user is unauthorized

x.x
[] All config checked during startup
    [] Check Repository compatibility with Entity
    []
[] Unit tests