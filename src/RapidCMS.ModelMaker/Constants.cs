namespace RapidCMS.ModelMaker
{
    public static class Constants
    {
        public static class Editors
        {
            public const string TextBox = "textbox";
            public const string TextArea = "textarea";
            public const string Dropdown = "dropdown";
            public const string Numeric = "numeric";
            public const string Checkbox = "checkbox";
            public const string Date = "date";
            public const string Select = "select";
            public const string MultiSelect = "multiselect";
        }

        public static class Validators
        {
            public const string MinLength = "minlength";
            public const string MaxLength = "maxlength";
            public const string LimitedOptions = "limitedOptions";
            public const string LinkedEntity = "linkedEntity";
        }

        public const string CollectionPrefix = "modelmaker";
        public const string ModelMakerAdminCollectionAlias = "modelmakeradmin";
    }
}
