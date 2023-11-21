using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Metadata;

namespace RapidCMS.Core.Forms;

internal class PropertyState
{
    private readonly List<string> _messages = new List<string>();
    private readonly List<string> _manualMessages = new List<string>();

    public PropertyState(IPropertyMetadata property)
    {
        Property = property ?? throw new ArgumentNullException(nameof(property));
    }

    public bool WasValidated { get; set; }
    public bool IsModified { get; set; }
    public bool IsBusy { get; set; }

    public IPropertyMetadata Property { get; }

    public IEnumerable<string> GetValidationMessages()
    {
        foreach (var message in _messages)
        {
            yield return message;
        }
        foreach (var message in _manualMessages)
        {
            yield return message;
        }
    }

    public void ClearMessages(bool clearManualMessages = false)
    {
        _messages.Clear();
        if (clearManualMessages)
        {
            _manualMessages.Clear();
        } 
    }

    public void AddMessage(string message, bool isManual = false)
    {
        if (isManual)
        {
            _manualMessages.Add(message);
        }
        else
        {
            _messages.Add(message);
        }

        WasValidated = true;
    }
}
