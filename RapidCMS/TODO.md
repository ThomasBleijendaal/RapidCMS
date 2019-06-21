1. Unique collection alias -> yes. TODO: enforce
2. Public or internal? -> internal all internals 

TODO:

1.0

[] Form validation
    [x] Validation on models / viewmodels
    [x] ValueMapper issues (nullable / non-nullable)
    [x] Move EditContext into UI class and remove UISubject
    [] Remove Name = "EditContext" from CascadingParameters
    [x] EditContext in CollectionRelation Select / Dropdowns is null
    [x] Allow for custom validation logic in Editor (for example: to prevent saving when upload is busy)
    [x] Nested EditContext does not work (validation is triggered, but not used)
    [x] Collection ignores button RequiresValidForm
    [x] Make relations work again
    [] Validation on BaseRelationEditor? -> Via EditContext?

[x] SetOneToManyRelation with collection 
    [] Must be able to pass parentId into Repository
    [x] DataCollection is transient helper class which contains the data for the relation (single or multiple entities)
    [x] Data form DataCollection passed to Insert and Update repository methods
        [x] During these save actions the relationship handling can be easily put in repository
    [x] New collection editor required for editing relation collection (different than subcollection)
    [x] Add + Remove buttons for adding and removing EXISTING entities 
    [] New CollectionDataProvider intermediate class handling all regular inter-collection relationships
    [] New InsertAndAdd button which inserts a new entity and adds it as relation -> not for multipleselect but for subcollection typed stuff
    [] DataCollection must support data from collection like CollectionRelation, but without the relation stuff
    [] Allow for setting ParentId in CollectionRelation for limiting entity selection
    [] Put RelationContainer in EditContext to make relations in Node + Collection working again

[x] Relation support (one-many + many-many via discrete call on Repository) 
    [] recursive delete 

[] Reordering
    [] Update to Blazor preview 6 to support @key on EditContext
        [] In listview: After updating entity 2 the EditContext of entity 1 is reset
        [] Put library statics in library (css / icons / js)
    [] Pagination (consistent during session)
        [] Redirect after deletion + consistent pagination / Return to parent button action
        [] Redirect after insertions + no update authorization goes to 403 page

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
[] Different set of collections for different entity variant (probably not needed)
[] IEntity.Id requirement is obstrusive
[] Editor visibility based upon predicates of current IEntity or evaluation by some object
[] Sub collection buttons connected to collection to prevent numerous buttons on each sub collection row (like Update All)
[] Extensible Dropdown (type ahead style insert of new elements + automatic update when used in list views)
[] ValueMapper support in NodeView and ListView (instead of hard string cast)

[] All config checked during startup
[] Unit tests