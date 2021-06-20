using System;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Enums
{
    [Flags]
    internal enum Relation
    {
        None = 0,

        One = 1,
        Many = 2,
        ToOne = 4,
        ToMany = 8,

        DependentSide = 1024
    }
}
