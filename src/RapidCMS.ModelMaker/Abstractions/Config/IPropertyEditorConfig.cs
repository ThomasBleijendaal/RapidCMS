using System;

namespace RapidCMS.ModelMaker.Abstractions.Config
{
    public interface IPropertyEditorConfig
    {
        string Alias { get; }
        string Name { get; }
        Type Editor { get; }
    }
}
