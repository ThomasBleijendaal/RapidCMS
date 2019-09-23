namespace TestLibrary.Entities
{
    public class PersonCountryEntity
    {
        public PersonEntity Person { get; set; }
        public int? PersonId { get; set; }

        public CountryEntity Country { get; set; }
        public int? CountryId { get; set; }
    }
}
