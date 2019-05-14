namespace TestLibrary.Entities
{
    public class CountryEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public PersonCountryEntity Person { get; set; }
    }
}
