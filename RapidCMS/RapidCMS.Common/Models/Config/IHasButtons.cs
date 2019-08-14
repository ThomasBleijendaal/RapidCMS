using System;
using RapidCMS.Common.Enums;

namespace RapidCMS.Common.Models.Config
{
    public interface IHasButtons<TReturn>
    {
        TReturn AddDefaultButton(DefaultButtonType type, string? label = null, string? icon = null, bool isPrimary = false);
        TReturn AddCustomButton<TActionHandler>(Type buttonType, string? label = null, string? icon = null);
    }
}
