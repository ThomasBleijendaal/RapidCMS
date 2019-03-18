using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Interfaces;
using System.Threading.Tasks;
using TestLibrary.Entities;
using TestLibrary.Repositories;

namespace TestServer.ActionHandlers
{
    public class CreateButtonActionHandler : IButtonActionHandler
    {
        private readonly AzureTableStorageRepository _repository;

        public CreateButtonActionHandler(AzureTableStorageRepository repository)
        {
            _repository = repository;
        }

        public CrudType GetCrudType()
        {
            return CrudType.None;
        }

        public async Task InvokeAsync()
        {
            var i = 0;

            do
            {
                await _repository.InsertAsync(null, new AzureTableStorageEntity()
                {
                    Description = $"New New New {i}",
                    Title = "Item {i}"
                });
            }
            while (++i < 10);
        }

        public bool IsCompatibleWithView(ViewContext viewContext)
        {
            return viewContext.Usage.HasFlag(UsageType.List);
        }
    }
}
