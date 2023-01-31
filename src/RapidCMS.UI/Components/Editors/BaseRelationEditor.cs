using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.UI.Components.Editors
{
    public abstract class BaseRelationEditor : BaseEditor
    {
        [Parameter] public IDataCollection? DataCollection { get; set; }

        public IRelationDataCollection? RelationDataCollection => DataCollection as IRelationDataCollection;

        protected override void AttachListener()
        {
            if (DataCollection != null)
            {
                DataCollection.OnDataChange += UpdateOptionsAsync;
            }
        }

        protected override void DetachListener()
        {
            if (DataCollection != null)
            {
                DataCollection.OnDataChange -= UpdateOptionsAsync;
            }
        }

        private void UpdateOptionsAsync(object? sender, EventArgs args) => UpdateOptionsAsync();

        protected abstract Task UpdateOptionsAsync();

        protected sealed override void Dispose()
        {
            base.Dispose();

            DataCollection?.Dispose();
            DataCollection = null;
        }
    }
}
