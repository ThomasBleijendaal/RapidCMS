﻿using System;
using System.Threading.Tasks;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Providers;
using RapidCMS.ModelMaker.Core.Abstractions.Factories;
using RapidCMS.ModelMaker.Core.Abstractions.Validation;

namespace RapidCMS.Example.ModelMaker.Factories
{
    public class EnumOptionsDataCollectionFactory<TEnum> : IDataCollectionFactory
        where TEnum : Enum
    {
        public Task<RelationSetup?> GetModelEditorRelationSetupAsync()
            // with a little imagination this method could return a ConcreteDataProviderRelationSetup
            // with a FixedOptionsDataProvider containing all the Enum types in the assembly
            // this will allow the user to select all types of enums when configuring the model
            => Task.FromResult(default(RelationSetup));

        public Task<RelationSetup?> GetModelRelationSetupAsync(IDetailConfig? config)
            // based on the selected enum (available in the config), this method could return
            // a specific EnumDataProvider for the selected enum
            => Task.FromResult<RelationSetup?>(
                new ConcreteDataProviderRelationSetup(
                    new EnumDataProvider<TEnum>()));
    }
}
