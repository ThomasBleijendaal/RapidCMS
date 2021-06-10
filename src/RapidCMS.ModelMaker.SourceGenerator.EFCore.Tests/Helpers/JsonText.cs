using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests.Helpers
{
    public class JsonText : AdditionalText
    {
        private readonly string _json;

        public JsonText(string json)
        {
            _json = json;
        }

        public override string Path => "./test.json";

        public override SourceText GetText(CancellationToken cancellationToken = default) => SourceText.From(_json);
    }
}
