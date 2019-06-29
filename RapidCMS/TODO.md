1. Unique collection alias -> yes. TODO: enforce
2. Public or internal? -> internal all internals 

TODO:

1.0

[] ManyToMany Collection Relation
    [] New IRepository methods
        -> void AddRelation<RelatedEntityType>(entityId, relatedEntityId)
        -> void RemoveRelation<RelatedEntityType>(entityId, relatedEntityId)
        -> IEnumerable<object> GetRelatedEntityIds<RelatedEntityType>(entityId)
    [] New Collection Page features
        -> ParentCollectionAlias + ParentId (for sub collection nesting)
        -> RelatedCollectionAlias + RelatedId (for related collection nesting)
    [] New Collection Page element
        -> Pick entity (top of tbody, using UI.RootEditContext, or UI.PickRelatedEntityEditContext)

[] Reordering
    [] New IRepository methods
        -> {unknown}
    [] Update to Blazor preview 6 to support @key on EditContext
        [] In listview: After updating entity 2 the EditContext of entity 1 is reset
    [] Pagination (consistent during session)
        [] Redirect after deletion + consistent pagination / Return to parent button action
        [] Redirect after insertions + no update authorization goes to 403 page

[] List Editor does not take missing editor in account

[] Custom Section support in Collection.razor (lists) (RowSection)

[] Button support everywhere (no more new List<Button>)
    [] ButtonActionHandler as generic parameter to CustomButton
    [] Bind CustomButton and ActionHandler more together

[] Property, Field, etc naming not totally consistent -> Refactor all names and methods

[] Top button bar should be side bar with meta
[] Navigation command during pre-render (server side rendering) fails
[] Use Blazor CSS parameter features
[] Date editor
[] Refresh data / relation collections upon save
    [] More events from EditContext and bind nested EditContext to root
    [] Make side bar tree react to collection actions (CRUD)
        [] Make possible to open sub collections with hidden root element
    [] OnBeforeUnload EditContext IsModified check

[] Merge NodeViewPane and NodeEditorPaneConfig

[] Implement Return everywhere + fallback to javascript:history.back();

1.1

[] Order of collection form configuration is not maintained

2.0
[] Repository parenId is brittle (is the parent the same entity type, or completely something different? must include parent entity type somehow)
    [] ParentId must be IEntity to support different types of nesting of repos
    [] Repository compatibility must be checked when using these nestings
    [] Buttons must be able to point to editors in other collections (view in collection-a, but edit in collection-b)
    [] Remove collection-alias and move to some other system
[] Different set of collections for different entity variant (probably not needed)
[] IEntity.Id requirement is obstrusive
[] Editor visibility based upon predicates of current IEntity or evaluation by some object
[] Sub collection buttons connected to collection to prevent numerous buttons on each sub collection row (like Update All)
[] Extensible Dropdown (type ahead style insert of new elements + automatic update when used in list views)
[] ValueMapper support in NodeView and ListView (instead of hard string cast)
[] Put library statics in library (css / icons / js)

x.x
[] All config checked during startup
    [] Check Repository compatibility with Entity
    []
[] Unit tests