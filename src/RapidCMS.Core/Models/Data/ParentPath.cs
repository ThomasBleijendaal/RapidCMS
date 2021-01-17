using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RapidCMS.Core.Models.Data
{
    public sealed class ParentPath : IReadOnlyCollection<(string repositoryAlias, string id)>
    {
        private readonly List<(string repositoryAlias, string id)> _path;

        private ParentPath(List<(string repositoryAlias, string id)> path)
        {
            _path = path ?? throw new ArgumentNullException(nameof(path));
        }

        private void Add(string repositoryAlias, string id)
        {
            _path.Add((repositoryAlias, id));
        }

        public static ParentPath? TryParse(string? path)
        {
            var list = path?.Split(";")
                .Select(chunk => chunk.Split(':'))
                .Where(x => x?.Count() == 2)
                .Select(x => (repositoryAlias: x[0], id: x[1]))
                .ToList();

            if (list == null || !list.Any())
            {
                return default;
            }

            return new ParentPath(list);
        }

        public static ParentPath AddLevel(ParentPath? currentPath, string repositoryAlias, string id)
        {
            var path = currentPath?.ToPathString();

            var newPath = TryParse(path) ?? new ParentPath(new List<(string repositoryAlias, string id)>());

            newPath.Add(repositoryAlias, id);

            return newPath;
        }

        public static (ParentPath newPath, string? repositoryAlias, string? id) RemoveLevel(ParentPath? currentPath)
        {
            if (currentPath == null)
            {
                return (new ParentPath(new List<(string repositoryAlias, string id)>()), null, null);
            }
            else
            {
                return (new ParentPath(currentPath._path.Take(currentPath._path.Count - 1).ToList()), currentPath._path.Last().repositoryAlias, currentPath._path.Last().id);
            }
        }

        public string ToPathString()
        {
            return string.Join(";", _path.Select(x => $"{x.repositoryAlias}:{x.id}"));
        }

        public IEnumerator<(string repositoryAlias, string id)> GetEnumerator()
        {
            return _path.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _path.GetEnumerator();
        }

        public int Count => _path.Count;
    }
}
