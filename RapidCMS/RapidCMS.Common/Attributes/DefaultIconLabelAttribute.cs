using System;

namespace RapidCMS.Common.Attributes
{
    internal class DefaultIconLabelAttribute : Attribute
    {
        public string Icon { get; set; }
        public string Label { get; set; }
    }

    internal class ActionsAttribute : Attribute
    {
        public ActionsAttribute(params string[] actions)
        {
            Actions = actions;
        }

        public string[] Actions { get; private set; }
    }
}
