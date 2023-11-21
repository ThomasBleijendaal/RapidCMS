using System;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.EventArgs.Mediators;

public class PaneResponseEventArgs : IMediatorResponseEventArgs<CrudType>
{
    public PaneResponseEventArgs(Guid requestId, CrudType response)
    {
        RequestId = requestId;
        Response = response;
    }

    public Guid RequestId { get; set; }
    public CrudType Response { get; set; }
}
