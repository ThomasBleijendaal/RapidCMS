using System.Linq;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Extensions
{
    public static class ParentPathExtensions
    {
        public static bool IsBaseOf(this ParentPath? parentPath, ParentPath? parentPathToCompare, string repositoryAlias, string? id)
        {
            if (parentPathToCompare == null || parentPath?.Count >= parentPathToCompare.Count)
            {
                return false;
            }

            if (parentPath == null)
            {
                var part = parentPathToCompare.ElementAtOrDefault(0);

                return CompareWithPart(repositoryAlias, id, part);
            }
            else
            {
                if (parentPathToCompare.Take(parentPath.Count).Except(parentPath).Any())
                {
                    return false;
                }
                else
                {
                    var part = parentPathToCompare.ElementAtOrDefault(parentPath.Count);

                    return CompareWithPart(repositoryAlias, id, part);
                } 
            }
        }

        private static bool CompareWithPart(string repositoryAlias, string? id, (string repositoryAlias, string id) part)
        {
            return id == null ? part.repositoryAlias == repositoryAlias : part == (repositoryAlias, id);
        }
    }
}
