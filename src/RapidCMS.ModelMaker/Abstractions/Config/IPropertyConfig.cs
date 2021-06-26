using System;
using System.Collections.Generic;

namespace RapidCMS.ModelMaker.Abstractions.Config
{
    public interface IPropertyConfig
    {
        string Alias { get; }
        string Name { get; }
        string Icon { get; }

        Type Type { get; }

        bool UsableAsTitle { get; }

        bool IsRelationToOne { get; }
        bool IsRelationToMany { get; }

        IList<IPropertyDetailConfig> Validators { get; }
        IList<IPropertyEditorConfig> Editors { get; }

        IPropertyConfig CanBeUsedAsTitle(bool usedAsTitle);
        IPropertyConfig RelatesToOneEntity(bool isRelation);
        IPropertyConfig RelatesToManyEntities(bool isRelation);
    }
}
