using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using RapidCMS.Common.ActionHandlers;
using RapidCMS.Common.Authorization;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Forms;
using RapidCMS.Common.Models;
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

        public Task ButtonClickAfterRepositoryActionAsync(Button button, EditContext editContext, ButtonContext context)
        {
            return Task.CompletedTask;
        }

        public async Task<CrudType> ButtonClickBeforeRepositoryActionAsync(Button button, EditContext editContext, ButtonContext context)
        {
            var i = 0;
            var max = Convert.ToInt64(context.CustomData);

            do
            {
                await _repository.InsertAsync(context.ParentId, new AzureTableStorageEntity()
                {
                    Description = $"New New New {i}",
                    Title = $"Item {i}"
                }, default);
            }
            while (++i < max);

            return CrudType.Refresh;
        }

        public OperationAuthorizationRequirement GetOperation(Button button, EditContext editContext)
        {
            return Operations.Create;
        }

        public bool IsCompatible(Button button, EditContext editContext)
        {
            return editContext.UsageType.HasFlag(UsageType.List);
        }

        public bool RequiresValidForm(Button button, EditContext editContext)
        {
            return false;
        }

        public bool ShouldAskForConfirmation(Button button, EditContext editContext)
        {
            return true;
        }
    }
}
