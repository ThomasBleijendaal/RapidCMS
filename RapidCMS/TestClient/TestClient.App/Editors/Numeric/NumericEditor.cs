namespace TestClient.App.Editors.Numeric
{
    public class NumericEditorBase : BaseEditor
    {
        protected override void OnParametersSet()
        {
            StateHasChanged();
        }

        public long Number
        {
            get
            {
                switch (Value)
                {
                    case long longValue:
                        return longValue;
                    case ulong ulongValue:
                        return (long)ulongValue;
                    case int intValue:
                        return intValue;
                    case uint uintValue:
                        return uintValue;
                    default:
                        return default;
                }
            }
            set => Value = value;
        }
    }
}
