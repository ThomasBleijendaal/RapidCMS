using System;
using System.Collections.Generic;

namespace RapidCMS.Core.Models.UI;

public class TreeNodesUI
{
    public TreeNodesUI(List<TreeNodeUI> nodes, bool moreDataAvailable)
    {
        Nodes = nodes ?? throw new ArgumentNullException(nameof(nodes));
        MoreDataAvailable = moreDataAvailable;
    }

    public List<TreeNodeUI> Nodes { get; set; }
    public bool MoreDataAvailable { get; set; }
}
