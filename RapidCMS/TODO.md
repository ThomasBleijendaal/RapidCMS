1. Unique collection alias -> yes. TODO: enforce
2. Public or internal?



TODO:

[x] Sticky upper button bar
[x] Make SetTreeView not required
[x] EditorPane general label
[x] ListEditorPaneConfig does not accept CustomButton
[x] Checkbox form field
[x] Nullable (numerics)
[] EditorValue obstrusiveness (weird get and set stuff)
[] Repository parenId is brittle (is the parent the same entity type, or completely something different? must include parent entity type somehow)
[] IEntity.Id requirement is obstrusive
[] SetOneToManyRelation with collection must be able to pass parentId into Repository
[] Extensible Dropdown (type ahead style insert of new elements + automatic update when used in list views)
[] ValueMapper is obstrusive (not able to cast int to long should not be an issue)
[] PropertyMetadata safe propagation
[] Recursive collection + check for recursion
[] Relation support (one-many + many-many via discrete call on Repository) + recursive delete + EFCore support (non-transient DbContext)
[] Optimized Generic Abstract EFCoreRepository
[] Pagination (consistent during session)
[] Form validation
[] Reordering
[] Editor visibility based upon predicates of current IEntity or evaluation by some object
[] Redirect after deletion + consistent pagination / Return to parent button action
[] AddSubCollectionListEditor should not create new pane (should be configured by user)
[x] Confirmation on button should make it danger
[] EnumDataTypeProvider support

[] All config checked during startup
[] Unit tests