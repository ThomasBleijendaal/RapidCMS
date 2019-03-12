1. Unique collection alias -> yes. TODO: enforce
2. Public or internal?



Main features

DONE: 1. basic editors
DONE: 2. sub collection list editor
DONE-ISH: 3. custom buttons support (TODO: add support in every form)
DONE-ISH: remove / reduce use of action in CollectionService and start using actionId of button. include custom buttons in this mechanism
DONE: 5. support for type differentation within collection 
DONE-ISH: 6. add support for list block editor (same as list editor, but then every line is a node editor) (TODO: delete from node editor redirects incorrect)
DONE: 7. move all cms UI to RapidCMS.UI

8. custom editor support
9. seperate view + edit node (now combined)
10. implement all default button types in all button call backs
11. check if (default) button is applicable EVERYWHERE
12. pagination + consistent pagination in list views and list editors
13. make better distinction between AddEditor + AddEditorPane
14. sort AddListPane out (what's the use of multipe list panes of the same entity? + add polymorphism)
15. support reordering
16. check after deletion where to redirect to (list view / list editor) (combine with pagination: support going back to previous page (via cancel button or delete button))
17. add and remove items (one to many + many to many relations)
18. add collection recursion
19. custom / default button action composition (update + return to list view)