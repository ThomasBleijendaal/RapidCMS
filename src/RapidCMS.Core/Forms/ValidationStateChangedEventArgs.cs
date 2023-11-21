namespace RapidCMS.Core.Forms;

public class ValidationStateChangedEventArgs
{
    public ValidationStateChangedEventArgs(bool? isValid = null)
    {
        IsValid = isValid;
    }

    public bool? IsValid { get; private set; }
}
