using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Plugins;
using RapidCMS.Core.Abstractions.Repositories;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.ModelMaker
{
    internal class ModelMakerPlugin : IPlugin
    {
        public string CollectionPrefix => "modelmaker";

        // TODO: async
        public CollectionSetup? GetCollection(string collectionAlias)
        {
            if (collectionAlias == "dynamic")
            {
                return _dynamic;
            }

            return null;
        }

        // TODO: async
        public IEnumerable<ITreeElementSetup> GetTreeElements()
        {
            return new ITreeElementSetup[]
            {
                // TODO: prefix should be added automatically
                new TreeElementSetup(_dynamic.Alias, PageType.Collection)
                {
                    RootVisibility = CollectionRootVisibility.Visible
                }
            };
        }

        public Type? GetRepository(string collectionAlias)
        {
            return typeof(ModelMakerRepository);
        }

        private CollectionSetup _dynamic = new CollectionSetup(
            "Database",
            "Gray100",
            "Dynamic Config",
            $"modelmaker::dynamic",
            $"modelmaker::dynamic",
            false,
            false)
        {
            
        };
    }

    internal class ModelMakerRepository : IRepository
    {
        public Task AddAsync(IRelated related, string id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string id, IParent? parent)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IEntity>> GetAllAsync(IParent? parent, IQuery query)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IEntity>> GetAllNonRelatedAsync(IRelated related, IQuery query)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IEntity>> GetAllRelatedAsync(IRelated related, IQuery query)
        {
            throw new NotImplementedException();
        }

        public Task<IEntity?> GetByIdAsync(string id, IParent? parent)
        {
            throw new NotImplementedException();
        }

        public Task<IEntity?> InsertAsync(IEditContext editContext)
        {
            throw new NotImplementedException();
        }

        public Task<IEntity> NewAsync(IParent? parent, Type? variantType)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(IRelated related, string id)
        {
            throw new NotImplementedException();
        }

        public Task ReorderAsync(string? beforeId, string id, IParent? parent)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(IEditContext editContext)
        {
            throw new NotImplementedException();
        }
    }
}
