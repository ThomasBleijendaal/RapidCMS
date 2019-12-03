namespace RapidCMS.Repositories
{
    public interface IConverter<TA, TB>
    {
        TA Convert(TB obj);
        TB Convert(TA obj);
    }
}
