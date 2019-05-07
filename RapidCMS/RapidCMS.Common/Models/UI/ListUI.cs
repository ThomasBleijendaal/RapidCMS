using System;
using System.Collections.Generic;
using RapidCMS.Common.Enums;

namespace RapidCMS.Common.Models.UI
{
    public class ListUI
    {
        public IEnumerable<UISubject> Entities { get; set; }

        public ListType ListType { get; set; }

        public List<ButtonUI> Buttons { get; set; }
        public Func<UISubject, SectionUI> SectionForEntity { get; set; }
    }
}
