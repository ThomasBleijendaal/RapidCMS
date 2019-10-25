TODO:

[] DisabledWhen
[] DisabledWhen(Entity.IsNew), DisabledWhen(Entity.IsExisiting)
[] VisibleWhen(Entity.IsNew), VisibleWhen(Entity.IsExisiting)
[] Nullability of models returned by repositories
[] Alignment checkboxes..
[] CustomLogins: Landing screen + Login state
[] DataProvider able to provide context for the entity (like tenant-id)
[] Escape from New

[] Order of collection form configuration is not maintained

[] Implement Return everywhere + fallback to javascript:history.back();

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