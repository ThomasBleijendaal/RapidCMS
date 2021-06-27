namespace RapidCMS.ModelMaker.Abstractions.Detail
{
    public interface IPropertyDetailModel<TDetailConfig>
        where TDetailConfig : IDetailConfig
    {
        TDetailConfig Config { get; }
    }
}
