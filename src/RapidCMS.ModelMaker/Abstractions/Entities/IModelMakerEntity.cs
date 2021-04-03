﻿using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.ModelMaker.Abstractions.Entities
{
    public interface IModelMakerEntity : IEntity
    {
        string Alias { get; set; }

        // TODO: implement state (Draft, Published, Changed)
    }
}