using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RapidCMS.Core.Models.Data
{
    public sealed class ParentPath : IEnumerable<(string collectionAlias, string id)>
    {
        private readonly List<(string collection, string id)> _path;

        private ParentPath(List<(string collection, string id)> path)
        {
            _path = path ?? throw new ArgumentNullException(nameof(path));
        }

        private void Add(string collection, string id)
        {
            _path.Add((collection, id));
        }

        public static ParentPath? TryParse(string? path)
        {
            var list = path?.Split(";")
                .Select(chunk => chunk.Split(':'))
                .Where(x => x?.Count() == 2)
                .Select(x => (collection: x[0], id: x[1]))
                .ToList();

            if (list == null || !list.Any())
            {
                return default;
            }

            return new ParentPath(list);
        }

        public static ParentPath AddLevel(ParentPath? currentPath, string collection, string id)
        {
            var path = currentPath?.ToPathString();

            var newPath = TryParse(path) ?? new ParentPath(new List<(string collection, string id)>());

            newPath.Add(collection, id);

            return newPath;
        }

        public static (ParentPath newPath, string? collectionAlias, string? id) RemoveLevel(ParentPath? currentPath)
        {
            if (currentPath == null)
            {
                return (new ParentPath(new List<(string collection, string id)>()), null, null);
            }
            else
            {
                return (new ParentPath(currentPath._path.Take(currentPath._path.Count - 1).ToList()), currentPath._path.Last().collection, currentPath._path.Last().id);
            }
        }

        public string ToPathString()
        {
            return string.Join(";", _path.Select(x => $"{x.collection}:{x.id}"));
        }

        public IEnumerator<(string collectionAlias, string id)> GetEnumerator()
        {
            return _path.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _path.GetEnumerator();
        }
    }
}
