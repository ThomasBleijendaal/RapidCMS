using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using RapidCMS.Common.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestLibrary.Entities;

#nullable enable

namespace TestLibrary.Repositories
{
    public class AzureTableStorageRepository : BaseClassRepository<string, string, AzureTableStorageEntity>
    {
        public const string DefaultParentId = "root";

        protected virtual string TableName => "rapidcmstable";

        private Task _initTask;
        private CloudTable? _table = null;

        public AzureTableStorageRepository(CloudStorageAccount cloudStorageAccount)
        {
            _initTask = Task.Run(async () =>
            {
                var client = cloudStorageAccount.CreateCloudTableClient();
                var table = client.GetTableReference(TableName);

                await table.CreateIfNotExistsAsync();

                var queuePermissions = new TablePermissions();
                queuePermissions.SharedAccessPolicies.Add(TableName, new SharedAccessTablePolicy { Permissions = SharedAccessTablePermissions.Query | SharedAccessTablePermissions.Add | SharedAccessTablePermissions.Update });
                await table.SetPermissionsAsync(queuePermissions);

                _table = table;
            });
        }

        public override async Task DeleteAsync(string id, string? parentId)
        {
            var entity = await GetByIdAsync(id, parentId);

            var op = TableOperation.Delete(entity);
            await _table!.ExecuteAsync(op);
        }

        public override async Task<IEnumerable<AzureTableStorageEntity>> GetAllAsync(string? parentId)
        {
            await _initTask;

            var q = new TableQuery<AzureTableStorageEntity>().Where(
                TableQuery.GenerateFilterCondition(nameof(AzureTableStorageEntity.PartitionKey), QueryComparisons.Equal, parentId ?? DefaultParentId));
            var data = await _table!.ExecuteQuerySegmentedAsync(q, null);

            return data.Results;
        }

        public override async Task<AzureTableStorageEntity> GetByIdAsync(string id, string? parentId)
        {
            await _initTask;

            var q = TableOperation.Retrieve<AzureTableStorageEntity>(parentId ?? DefaultParentId, id);
            var data = await _table!.ExecuteAsync(q);

            if (data.Result is AzureTableStorageEntity entity)
            {
                return entity;
            }
            else
            {
                return null;
            }
        }

        public override async Task<AzureTableStorageEntity> InsertAsync(string? parentId, AzureTableStorageEntity entity)
        {
            await _initTask;

            entity.Id = Guid.NewGuid().ToString();
            entity.PartitionKey = parentId ?? DefaultParentId;

            var op = TableOperation.Insert(entity);
            var data = await _table!.ExecuteAsync(op);

            if (data.Result is AzureTableStorageEntity updatedEntity)
            {
                return updatedEntity;
            }
            else
            {
                return null;
            }
        }

        public override async Task<AzureTableStorageEntity> NewAsync(string? parentId, Type? variantType)
        {
            await _initTask;

            return new AzureTableStorageEntity { PartitionKey = parentId ?? DefaultParentId };
        }

        public override async Task UpdateAsync(string id, string? parentId, AzureTableStorageEntity entity)
        {
            await _initTask;

            entity.Id = id;
            entity.PartitionKey = parentId ?? DefaultParentId;

            var op = TableOperation.InsertOrReplace(entity);
            await _table!.ExecuteAsync(op);
        }

        public override string ParseKey(string id)
        {
            return id;
        }

        public override string? ParseParentKey(string? parentId)
        {
            return parentId;
        }
    }
}
