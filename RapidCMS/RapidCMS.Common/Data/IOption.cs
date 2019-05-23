namespace RapidCMS.Common.Data
{
    public interface IElement
    {
        object Id { get; }
        
        // TODO: make label columnable (support for multiple columns in UI)
        string Label { get; }
    }
}
