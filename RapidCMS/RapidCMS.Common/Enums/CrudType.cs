namespace RapidCMS.Common.Enums
{
    public enum CrudType
    {
        // none
        None = 0,

        // viewers
        View = 1,
        // TODO: merge list with view
        List = 1,
        Create = 2,
        Read = 3,

        // modifyers
        Insert = 4,
        Update = 5,
        Delete = 6,

        // list action
        Add = 100,
        Remove = 101,

        // specials
        Refresh = 1001
    }
}
