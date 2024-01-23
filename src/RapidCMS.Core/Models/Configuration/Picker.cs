namespace RapidCMS.Core.Models.Configuration;

public record Picker(
    bool EnableSelectAll = false,
    bool EnableUnselectAll = false,
    bool EnableReset = false,
    int PageSize = 25);
