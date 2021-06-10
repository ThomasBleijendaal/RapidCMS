using System;
using RapidCMS.ModelMaker.Enums;

namespace RapidCMS.ModelMaker.Abstractions.Entities
{
    public interface IPublishableModelMakerEntity : IModelMakerEntity
    {
        PublishState State { get; set; }

        DateTime CreatedAt { get; set; }

        DateTime PublishedAt { get; set; }

        DateTime UpdatedAt { get; set; }
    }
}
