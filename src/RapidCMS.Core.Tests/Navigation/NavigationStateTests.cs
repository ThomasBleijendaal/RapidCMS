using NUnit.Framework;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Navigation;
using System.Collections;

namespace RapidCMS.Core.Tests.Navigation;

public class NavigationStateTests
{
    [TestCaseSource(typeof(NavigationStateTestCases))]
    public void WhenUrlIsGiven_ThenUrlIsParsed(string url, string query, NavigationState expectedState)
    {
        var state = new NavigationState(url, query);

        Assert.That(expectedState.Equals(state), Is.True);
    }

    private class NavigationStateTestCases : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            yield return new object[] { 
                "/collection/edit/person/",
                "",
                new NavigationState("person", default(ParentPath), UsageType.Edit)
            };

            yield return new object[] {
                "/collection/edit/person/",
                "?p=1",
                new NavigationState("person", default(ParentPath), UsageType.Edit)
                {
                    CollectionState = new CollectionState(null, null, 1, null)
                }
            };

            yield return new object[]
            {
                "/node/edit/person-convention/VNRDry/-/428281356/",
                "",
                new NavigationState("person-convention", default(ParentPath), "VNRDry", "428281356", UsageType.Edit)
            };

            yield return new object[]
            {
                "/node/edit/person-convention/VNRDry/fdsa:123/428281356/",
                "",
                new NavigationState("person-convention", ParentPath.TryParse("fdsa:123"), "VNRDry", "428281356", UsageType.Edit)
            };

            yield return new object[]
            {
                "/node/edit/person-convention/VNRDry/fdsa:123;fdsafdsa:12345/428281356/",
                "",
                new NavigationState("person-convention", ParentPath.TryParse("fdsa:123;fdsafdsa:12345"), "VNRDry", "428281356", UsageType.Edit)
            };

            yield return new object[]
            {
                "node/new/variants/aOkLV_",
                "",
                new NavigationState("variants", default(ParentPath), "aOkLV_", default(string), UsageType.New)
            };

            yield return new object[]
            {
                "/node/new/person/yY4Nc6/Da9Ic3-Sv5oVVgQDwmEuxhfvCEs7j6maatAe46OuNgA:439326248;Da9Ic3-Sv5oVVgQDwmEuxhfvCEs7j6maatAe46OuNgA:654154684;Da9Ic3-Sv5oVVgQDwmEuxhfvCEs7j6maatAe46OuNgA:1594109173/",
                "",
                new NavigationState(
                    "person",
                    ParentPath.TryParse("Da9Ic3-Sv5oVVgQDwmEuxhfvCEs7j6maatAe46OuNgA:439326248;Da9Ic3-Sv5oVVgQDwmEuxhfvCEs7j6maatAe46OuNgA:654154684;Da9Ic3-Sv5oVVgQDwmEuxhfvCEs7j6maatAe46OuNgA:1594109173"),
                    "yY4Nc6",
                    default(string),
                    UsageType.New)
            };
        }
    }
}
