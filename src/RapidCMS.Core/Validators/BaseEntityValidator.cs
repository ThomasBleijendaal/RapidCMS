﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Abstractions.Validators;

namespace RapidCMS.Core.Validators
{
    public abstract class BaseEntityValidator<TEntity> : IEntityValidator
        where TEntity : IEntity
    {
        protected abstract IEnumerable<ValidationResult> Validate(TEntity entity, IRelationContainer relationContainer);

        IEnumerable<ValidationResult> IEntityValidator.Validate(IEntity entity, IRelationContainer relationContainer) => Validate((TEntity)entity, relationContainer);
    }
}