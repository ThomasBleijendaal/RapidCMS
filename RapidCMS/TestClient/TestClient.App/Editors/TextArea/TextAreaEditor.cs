using Microsoft.AspNetCore.Components;

namespace TestClient.App.Editors.TextArea
{
    public class TextAreaEditorBase : BaseEditor
    {
        public string Text
        {
            get => Value.ToString();
            set => Value = value;
        }
    }
}
