//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using RapidCMS.Core.Abstractions.Data;
//using RapidCMS.Core.Forms;
//using RapidCMS.ModelMaker.Abstractions.DataCollections;
//using RapidCMS.ModelMaker.Abstractions.Validation;

//namespace RapidCMS.ModelMaker.DataCollections
//{
//    public abstract class PropertyDataCollection<TConfigType> : IDataCollectionFactory
//        where TConfigType : class, IValidatorConfig
//    {
//        public event EventHandler? OnDataChange;

//        public abstract void Dispose();

//        public abstract Task<IEnumerable<IElement>> GetAvailableElementsAsync();

//        protected abstract Task SetConfigAsync(TConfigType? config);

//        public Task SetConfigAsync(IValidatorConfig? config) => SetConfigAsync(config as TConfigType);

//        protected abstract Task ResolveDependenciesAsync(IServiceProvider serviceProvider);

//        protected abstract Task SetModelMakerEntityAsync(FormEditContext editContext, IParent? parent);

//        public async Task SetEntityAsync(FormEditContext editContext, IParent? parent)
//        {
//            await ResolveDependenciesAsync(editContext.FormState.ServiceProvider);
//            await SetModelMakerEntityAsync(editContext, parent);
//        }

//        public abstract Task<IDataCollection> GetModelEditorDataCollectionAsync();
//    }
//}
