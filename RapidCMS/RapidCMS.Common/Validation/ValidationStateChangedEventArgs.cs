namespace RapidCMS.Common.Validation
{
    public class ValidationStateChangedEventArgs
    {
        public ValidationStateChangedEventArgs(bool isValid)
        {
            IsValid = isValid;
        }

        public bool IsValid { get; private set; }
    }
}
