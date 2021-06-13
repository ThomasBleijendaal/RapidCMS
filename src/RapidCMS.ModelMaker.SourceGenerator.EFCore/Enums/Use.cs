using System;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Enums
{
    [Flags]
    internal enum Use
    {
        Entity = 1,
        Collection = 2,
        Context = 4,
        Repository = 8
    }
}
