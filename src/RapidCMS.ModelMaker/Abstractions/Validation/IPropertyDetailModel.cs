namespace RapidCMS.ModelMaker.Core.Abstractions.Validation
{
    public interface IPropertyDetailModel<TDetailConfig>
        where TDetailConfig : IDetailConfig
    {
        TDetailConfig Config { get; }
    }
}
