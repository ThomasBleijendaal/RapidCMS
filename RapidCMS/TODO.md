1. Unique collection alias -> yes. TODO: enforce
2. Public or internal? -> internal all internals 

TODO:

1.0

[x] SetOneToManyRelation with collection 
    [x] Must be able to pass parentId into Repository
    [x] DataCollection is transient helper class which contains the data for the relation (single or multiple entities)
    [x] Data form DataCollection passed to Insert and Update repository methods
        [x] During these save actions the relationship handling can be easily put in repository
    [x] New collection editor required for editing relation collection (different than subcollection)
    [x] Add + Remove buttons for adding and removing EXISTING entities 
    [x] New CollectionDataProvider intermediate class handling all regular inter-collection relationships
    [] New InsertAndAdd button which inserts a new entity and adds it as relation 
        -> not for multipleselect but for subcollection typed stuff
    [x] DataCollection must support data from collection like CollectionRelation, but without the relation stuff
    [x] Allow for setting ParentId in CollectionRelation for limiting entity selection
    [x] Put RelationContainer in EditContext to make relations in Node + Collection working again

[] BaseEditor SetValue not non-nullable safe

[x] Relation support (one-many + many-many via discrete call on Repository) 
    [] recursive delete 

[] Reordering
    [] Update to Blazor preview 6 to support @key on EditContext
        [] In listview: After updating entity 2 the EditContext of entity 1 is reset
        [] Put library statics in library (css / icons / js)
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

1.1

[] Order of collection form configuration is not maintained

2.0
[] Repository parenId is brittle (is the parent the same entity type, or completely something different? must include parent entity type somehow)
    [] ParentId must be IEntity to support different types of nesting of repos
    [] Repository compatibility must be checked when using these nestings
[] Different set of collections for different entity variant (probably not needed)
[] IEntity.Id requirement is obstrusive
[] Editor visibility based upon predicates of current IEntity or evaluation by some object
[] Sub collection buttons connected to collection to prevent numerous buttons on each sub collection row (like Update All)
[] Extensible Dropdown (type ahead style insert of new elements + automatic update when used in list views)
[] ValueMapper support in NodeView and ListView (instead of hard string cast)

x.x
[] All config checked during startup
    [] Check Repository compatibility with Entity
    []
[] Unit tests