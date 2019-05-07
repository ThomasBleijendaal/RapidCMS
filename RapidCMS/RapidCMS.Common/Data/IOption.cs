namespace RapidCMS.Common.Data
{
    public interface IOption
    {
        string Id { get; }
        
        // TODO: make label columnable (support for multiple columns in UI)
        string Label { get; }
    }
}
