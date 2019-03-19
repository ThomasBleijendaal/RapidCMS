namespace RapidCMS.Common.Enums
{
    public enum CrudType
    {
        // none
        None = 0,

        // viewers
        View = 1,
        Create = 2,
        Read = 3,

        // modifyers
        Insert = 4,
        Update = 5,
        Delete = 6,

        // specials
        Refresh = 1001
    }
}
