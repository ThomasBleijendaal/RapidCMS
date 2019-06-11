using System;


namespace RapidCMS.Common.Models
{
    public class CustomTypeRegistration
    {
        internal CustomTypeRegistration(Type type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Alias = type.FullName;
        }

        public Type Type { get; set; }
        public string Alias { get; set; }
    }
}
