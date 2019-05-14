using System;
using System.Collections.Generic;
using System.Text;

namespace TestLibrary.Entities
{
    public class PersonEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public PersonCountryEntity Country { get; set; }
    }
}
