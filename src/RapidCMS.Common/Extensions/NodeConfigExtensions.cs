using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Config;

namespace RapidCMS.Common.Extensions
{
    internal static class NodeConfigExtensions
    {
        public static Node ToNode(this NodeConfig node, Collection collection)
        {
            return new Node
            {
                Buttons = node.Buttons.ToList(button => button.ToButton(collection.SubEntityVariants, collection.EntityVariant)),
                BaseType = node.BaseType,
                Panes = node.Panes.ToList(config => config.ToPane())
            };
        }
    }
}
