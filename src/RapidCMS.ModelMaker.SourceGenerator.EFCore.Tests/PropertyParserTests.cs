using System;
using System.Collections.Generic;
using NUnit.Framework;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Parsers;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests
{
    public class PropertyParserTests
    {
        [TestCase(typeof(string), "System.String")]
        [TestCase(typeof(List<string>), "System.Collections.Generic.List<System.String>")]
        [TestCase(typeof(Dictionary<string, string>), "System.Collections.Generic.Dictionary<System.String, System.String>")]
        [TestCase(typeof(Dictionary<List<string>, string>), "System.Collections.Generic.Dictionary<System.Collections.Generic.List<System.String>, System.String>")]
        [TestCase(typeof(Dictionary<List<string>, List<string>>), "System.Collections.Generic.Dictionary<System.Collections.Generic.List<System.String>, System.Collections.Generic.List<System.String>>")]
        [TestCase(typeof(List<Dictionary<List<string>, List<string>>>), "System.Collections.Generic.List<System.Collections.Generic.Dictionary<System.Collections.Generic.List<System.String>, System.Collections.Generic.List<System.String>>>")]
        [TestCase(typeof(Dictionary<Dictionary<string, string>, List<Dictionary<string, string>>>), "System.Collections.Generic.Dictionary<System.Collections.Generic.Dictionary<System.String, System.String>, System.Collections.Generic.List<System.Collections.Generic.Dictionary<System.String, System.String>>>")]
        public void WhenTypeFullNameIsGivenToPropertyParse_ThenCorrectCSharpTypeNotationIsGenerated(Type type, string expectedNotation)
        {
            var notation = new PropertyParser().ParseTypeWithNamespace(type.FullName);

            Assert.AreEqual(expectedNotation, notation);
        }
    }
}
