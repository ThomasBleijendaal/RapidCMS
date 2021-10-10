using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.ModelMaker.DataCollections
{
    internal class CollectionsDataCollection : IDataCollection
    {
        private readonly ISetupResolver<IEnumerable<ITreeElementSetup>> _setupResolver;

        public CollectionsDataCollection(ISetupResolver<IEnumerable<ITreeElementSetup>> setupResolver)
        {
            _setupResolver = setupResolver;
        }

        public void Configure(object configuration) { }

        public event EventHandler? OnDataChange;

        public void Dispose()
        {
        }

        public async Task<IReadOnlyList<IElement>> GetAvailableElementsAsync(IView view)
        {
            var treeElements = await _setupResolver.ResolveSetupAsync();

            return treeElements
                .Where(treeElement => treeElement.Type == PageType.Collection)
                .Where(treeElement => treeElement.Alias != Constants.ModelMakerAdminCollectionAlias)
                .Select(treeElement => new Element
                {
                    Id = treeElement.Alias,
                    Labels = new[] { treeElement.Name }
                })
                .ToList();
        }

        public Task SetEntityAsync(FormEditContext editContext, IParent? parent)
        {
            return Task.CompletedTask;
        }
    }
}
