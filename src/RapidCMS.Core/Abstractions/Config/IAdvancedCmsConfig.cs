using System;

namespace RapidCMS.Core.Abstractions.Config
{
    public interface IAdvancedCmsConfig
    {
        int SemaphoreCount { get; set; }
    }
}
