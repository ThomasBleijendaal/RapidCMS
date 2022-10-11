using BlazorMonaco;

namespace RapidCMS.Extensions.Monaco;

/// <summary>
/// Must be a new instance and not shared between Monaco Editor instances.
/// </summary>
/// <param name="Options"></param>
public record MonacoEditorConfiguration(StandaloneEditorConstructionOptions? Options);
