namespace RapidCMS.Core.Enums
{
    public enum CrudType
    {
        // none
        None = 0,

        // viewers
        View = 1,
        List = 1,
        Create = 2,
        Edit = 3,

        // modifiers
        Insert = 4,
        Update = 5,
        Delete = 6,

        // list action
        Add = 100,
        Remove = 101,
        Pick = 102,

        // navigation actions
        Return = 1000,
        Refresh = 1001,
        Up = 1002
    }
}
