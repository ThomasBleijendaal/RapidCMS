using System;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Common.Data;

namespace RapidCMS.Example.Data
{
    public class User : IEntity, ICloneable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }

        [Required]
        // the horror
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", 
            ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        public string Password { get; set; }

        string IEntity.Id { get => Id.ToString(); set => Id = int.Parse(value); }

        public object Clone()
        {
            return new User
            {
                Id = Id,
                Name = Name,
                Password = Password
            };
        }
    }
}
