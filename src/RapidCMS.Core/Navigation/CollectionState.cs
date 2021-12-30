using RapidCMS.Core.Helpers;

namespace RapidCMS.Core.Navigation
{
    public record CollectionState(
        int? ActiveTab = default,
        string? SearchTerm = default,
        int CurrentPage = 1,
        int? MaxPage = default)
    {
        public override string ToString()
        {
            return UriHelper.CombineQueryString(
                ("tab", ActiveTab?.ToString()),
                ("q", SearchTerm),
                ("p", CurrentPage == 1 ? null : CurrentPage.ToString()));
        }
    }
}
