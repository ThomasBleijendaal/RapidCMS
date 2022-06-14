using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Navigation
{
    public class SortBag 
    {
        private readonly Dictionary<int, OrderByType> _sorts = new();

        public SortBag()
        {

        }

        public SortBag(IEnumerable<KeyValuePair<int, OrderByType>> sorts)
        {
            foreach (var sort in sorts)
            {
                _sorts[sort.Key] = sort.Value;
            }
        }

        public OrderByType? Get(int index) => _sorts.ContainsKey(index) ? _sorts[index] : null;

        public SortBag Clear() => new SortBag();

        public SortBag Add(int index, OrderByType direction) => new SortBag(_sorts.Append(new KeyValuePair<int, OrderByType>(index, direction)));

        public override string ToString() => $"{string.Join("--", _sorts.Select(x => $"{x.Key}-{x.Value}"))}".ToLower();
    }
}
