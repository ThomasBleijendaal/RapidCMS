TODO:

[] Documentation + Examples

[v] Allow for messaging (saved successfully etc)

[v] Allow for side panes
[v] Custom modal / form based on button click
    - Make it as 'delayed'-button (once modal closes, the crud type is determined)
    - Everything inside modal is custom

[] Order of collection form configuration is not maintained
  
[] Pull all externally inheritable UI elements into RapidCMS.UI.Common

[v] Refresh data / relation collections upon save
    [v] More events from EditContext and bind nested EditContext to root
    [v] Make side bar tree react to collection actions (CRUD)
        [v] Make possible to open sub collections with hidden root element
    [] OnBeforeUnload EditContext IsModified check

[] Implement Return everywhere + fallback to javascript:history.back();

[v] Button support everywhere (no more new List<Button>)

[] Navigation command during pre-render (server side rendering) fails

2.0
[] Add more than one view to page (to allow for ListView + CustomBlock)
[] Repository parenId is brittle (is the parent the same entity type, or completely something different? must include parent entity type somehow)
    [] ParentId must be IEntity to support different types of nesting of repos
    [] Merge RelationCollection + Collection (RelatedEntity + ParentEntity (instead of parentId))
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
[] Reordering

x.x
[] All config checked during startup
    [] Check Repository compatibility with Entity
    []
[] Unit tests