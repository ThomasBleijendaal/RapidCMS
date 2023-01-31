using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.UI.Components.Editors
{
    public abstract class BaseDataEditor : BasePropertyEditor
    {
        [Parameter] public IDataCollection? DataCollection { get; set; }

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
