using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Metadata;
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

        public Type? GetRepositoryType(string collectionAlias)
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
            UsageType = UsageType.List,
            TreeView = new TreeViewSetup(EntityVisibilty.Visible, CollectionRootVisibility.Visible, false, false, new ModelMakerEntityExpressionMetadata("Name", x => x.Data["Name"])),
            ListView = new ListSetup(100, false, false, ListType.Table, EmptyVariantColumnVisibility.Collapse, new List<PaneSetup>(), new List<IButtonSetup>())
        };
    }

    internal class ModelMakerEntityExpressionMetadata : IExpressionMetadata
    {
        public ModelMakerEntityExpressionMetadata(string name, Func<ModelMakerEntity, string> getter)
        {
            PropertyName = name;
            StringGetter = x => getter.Invoke((ModelMakerEntity)x);
        }

        public string PropertyName { get; }

        public Func<object, string> StringGetter { get; }
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
            return Task.FromResult<IEnumerable<IEntity>>(new[] {
                new ModelMakerEntity
                {
                    Id = "1",
                    Data = new Dictionary<string, string>
                    {
                        { "Name", "Name1" }
                    }
                },
                new ModelMakerEntity
                {
                    Id = "2",
                    Data = new Dictionary<string, string>
                    {
                        { "Name", "Name2" }
                    }
                }
            });
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
            return Task.FromResult<IEntity>(new ModelMakerEntity());
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

    internal class ModelMakerEntity : IEntity
    {
        public string? Id { get; set; }

        public Dictionary<string, string> Data { get; set; } = default!;
    }


}
