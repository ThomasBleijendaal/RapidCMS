1. Unique collection alias -> yes. TODO: enforce
2. Public or internal? -> internal all internals 



TODO:

1.0
[x] Add NodeView to setup to allow for explicit node viewing configurations (node editor is not easily convertable)
    [] Test SubCollection List view 
[] Reordering
[] Form validation
    [] Validation on models / viewmodels
[] Pagination (consistent during session)
    [] Redirect after deletion + consistent pagination / Return to parent button action
    [] Redirect after insertions + no update authorization goes to 403 page

[] Custom Section support in Collection.razor (lists) (RowSection)
[] Button support everywhere (no more new List<Button>)
[] Property, Field, etc naming not totally consistent -> Refactor all names and methods
[x] SetOneToManyRelation with collection 
    - Must be able to pass parentId into Repository
    [x] DataCollection is transient helper class which contains the data for the relation (single or multiple entities)
    [x] Data form DataCollection passed to Insert and Update repository methods
        [x] During these save actions the relationship handling can be easily put in repository
    [x] New collection editor required for editing relation collection (different than subcollection)
    [x] Add + Remove buttons for adding and removing EXISTING entities 
    [] New CollectionDataProvider intermediate class handling all regular inter-collection relationships
    [] New InsertAndAdd button which inserts a new entity and adds it as relation -> not for multipleselect but for subcollection typed stuff
[] Extensible Dropdown (type ahead style insert of new elements + automatic update when used in list views)
[] ValueMapper is obstrusive (not able to cast int to long should not be an issue)
    [] Find solution for ValueMapper use in ListViewConfig -> automatic valuemapper selection for given type + add it back to config + call it DisplayMapper
    [] Explore if ValueMapper can be skipped since editors are using late resolving of value -> cannot be skipped in ListViewConfig but should be skipped in editors
[x] Relation support (one-many + many-many via discrete call on Repository) 
    [] recursive delete 
    [] EFCore support (non-transient DbContext)
[] AddSubCollectionListEditor should not create new pane (should be configured by user)
[] EnumDataTypeProvider support
[] Top button bar should be side bar with meta
[] Allow for IExpressionMetadata is list views
[] Recursive collection + check for recursion
[] Navigation command during pre-render (server side rendering) fails

2.0
[] Repository parenId is brittle (is the parent the same entity type, or completely something different? must include parent entity type somehow)
[] Different set of collections for different entity variant (probably not needed)
[] Bind CustomButton and ActionHandler more together
[] IEntity.Id requirement is obstrusive
[] Editor visibility based upon predicates of current IEntity or evaluation by some object

[] All config checked during startup
[] Unit tests