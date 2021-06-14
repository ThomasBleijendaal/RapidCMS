using System;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.UI.PublishableEntity.Enums;

namespace RapidCMS.UI.PublishableEntity.Abstractions
{
    public interface IPublishableModelMakerEntity : IEntity
    {
        PublishState State { get; set; }

        DateTime CreatedAt { get; set; }

        DateTime PublishedAt { get; set; }

        DateTime UpdatedAt { get; set; }
    }
}
