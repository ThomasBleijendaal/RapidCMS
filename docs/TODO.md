TODO:

[] Order of collection form configuration is not maintained

[] Implement Return everywhere + fallback to javascript:history.back();

[] Navigation command during pre-render (server side rendering) fails

2.0
[] Add more than one view to page (to allow for ListView + CustomBlock)
[] Repository parenId is brittle (is the parent the same entity type, or completely something different? must include parent entity type somehow)
    [] ParentId must be IEntity to support different types of nesting of repos
    [] Merge RelationCollection + Collection (RelatedEntity + ParentEntity (instead of parentId))
    [] Repository compatibility must be checked when using these nestings
    [] Buttons must be able to point to editors in other collections (view in collection-a, but edit in collection-b)
    [] Remove collection-alias and move to some other system
[] IEntity.Id requirement is obstrusive
[] Extensible Dropdown (type ahead style insert of new elements + automatic update when used in list views)
[] GetRelationListViewAsync method evaluates authorization for all buttons to relatedEntity, while the processing functions will evaluate authorizaton sometimes to IEntity and sometimes to IRelatedEntity.
[] Hide sections of the tree when user is unauthorized
[] Reordering

x.x
[] All config checked during startup
    [] Check Repository compatibility with Entity
    []
[] Unit tests