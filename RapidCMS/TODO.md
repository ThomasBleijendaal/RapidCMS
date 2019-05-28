1. Unique collection alias -> yes. TODO: enforce
2. Public or internal? -> internal all internals 



TODO:

[x] Sticky upper button bar
[x] Make SetTreeView not required
[x] EditorPane general label
[x] ListEditorPaneConfig does not accept CustomButton
[x] Checkbox form field
[x] Nullable (numerics)
[x] EditorValue obstrusiveness (weird get and set stuff)
[] Repository parenId is brittle (is the parent the same entity type, or completely something different? must include parent entity type somehow)
[] IEntity.Id requirement is obstrusive
[x] SetOneToManyRelation with collection must be able to pass parentId into Repository
    - DataCollection is transient helper class which contains the data for the relation (single or multiple entities)
    - Data form DataCollection passed to Insert and Update repository methods
        - During these save actions the relationship handling can be easily put in repository
    - New collection editor required for editing relation collection (different than subcollection)
    - Add + Remove buttons for adding and removing EXISTING entities, and new InsertAndAdd button which inserts a new entity and adds it as relation
    - New CollectionDataProvider intermediate class handling all regular inter-collection relationships
[] Extensible Dropdown (type ahead style insert of new elements + automatic update when used in list views)
[] ValueMapper is obstrusive (not able to cast int to long should not be an issue)
[] Explore if ValueMapper can be skipped since editors are using late resolving of value
[] Recursive collection + check for recursion
[x] Relation support (one-many + many-many via discrete call on Repository) 
    [] recursive delete 
    [] EFCore support (non-transient DbContext)
[] Optimized Generic Abstract EFCoreRepository
[] Pagination (consistent during session)
[] Form validation
[] Reordering
[] Editor visibility based upon predicates of current IEntity or evaluation by some object
[] Redirect after deletion + consistent pagination / Return to parent button action
[] AddSubCollectionListEditor should not create new pane (should be configured by user)
[x] Confirmation on button should make it danger
[] EnumDataTypeProvider support
[] Top button bar should be side bar with meta
[] Bind CustomButton and ActionHandler more together
[] Allow for IExpressionMetadata is list views
[] Property, Field, etc naming not totally consistent

[] All config checked during startup
[] Unit tests