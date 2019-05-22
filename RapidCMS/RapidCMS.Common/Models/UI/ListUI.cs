using System;
using System.Collections.Generic;
using RapidCMS.Common.Enums;

namespace RapidCMS.Common.Models.UI
{
    public class ListUI
    {
        public IEnumerable<UISubject> Entities { get; internal set; }

        public ListType ListType { get; internal set; }

        public List<ButtonUI> Buttons { get; internal set; }

        // TODO: this could return multiple items
        public Func<UISubject, SectionUI> SectionForEntity { get; internal set; }
    }
}
