using System.Collections.Generic;
using System.Text.RegularExpressions;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Enums;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Information
{
    internal abstract class InformationBase
    {
        protected readonly List<(Use use, string @namespace)> _namespaces = new();

        protected string ValidPascalCaseName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "";
            }
            else
            {
                var trimmedName = Regex.Replace(name?.Trim() ?? "", "[^A-Za-z0-9]*", "");

                if (trimmedName.Length == 0)
                {
                    return "";
                }

                if (trimmedName.Length == 1)
                {
                    return char.ToUpper(trimmedName[0]).ToString();
                }

                return $"{char.ToUpper(trimmedName[0])}{trimmedName.Substring(1)}";
            }
        }
    }
}
