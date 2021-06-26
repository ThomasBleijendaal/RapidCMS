using System;

namespace RapidCMS.ModelMaker.Enums
{
    [Flags]
    public enum OutputItem
    {
        Entity = 1,
        Collection = 2,
        Repository = 4,
        Context = 8,
        Validation = 16
    }
}
