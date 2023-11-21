using System;

namespace RapidCMS.Core.Models.EventArgs;

public sealed class RowDragEventArgs
{
    public RowDragEventArgs(string subjectId, string? targetId)
    {
        SubjectId = subjectId ?? throw new ArgumentNullException(nameof(subjectId));
        TargetId = targetId;
    }

    public string SubjectId { get; }
    public string? TargetId { get; }
}
