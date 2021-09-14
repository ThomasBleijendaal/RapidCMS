using System;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Repositories;

namespace RapidCMS.Example.Shared.Data
{
    public class RandomRepo : JsonRepository<Person>
    {
        public RandomRepo(IMediator mediator, IServiceProvider serviceProvider) : base(mediator, serviceProvider)
        {
        }
    }
}
