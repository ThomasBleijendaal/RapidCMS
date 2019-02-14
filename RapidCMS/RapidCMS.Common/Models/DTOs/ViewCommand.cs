using System;
using System.Collections.Generic;
using System.Text;

namespace RapidCMS.Common.Models.DTOs
{
    public abstract class ViewCommand
    {

    }

    public class NavigateCommand : ViewCommand
    {
        public string Uri { get; set; }
    }
}
