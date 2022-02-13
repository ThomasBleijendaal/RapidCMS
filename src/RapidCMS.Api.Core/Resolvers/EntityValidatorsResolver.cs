using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Api.Core.Resolvers
{
    internal class ValidationSetupResolver : ISetupResolver<IReadOnlyList<ValidationSetup>>
    {
        private readonly Dictionary<string, IReadOnlyList<ValidationSetup>> _validations;

        public ValidationSetupResolver(Dictionary<string, IReadOnlyList<ValidationSetup>> validations)
        {
            _validations = validations;
        }

        public Task<IReadOnlyList<ValidationSetup>> ResolveSetupAsync()
            => Task.FromResult<IReadOnlyList<ValidationSetup>>(_validations.Values.SelectMany(x => x).ToList());

        public Task<IReadOnlyList<ValidationSetup>> ResolveSetupAsync(string alias)
            => Task.FromResult(_validations.TryGetValue(alias, out var list) ? list : new List<ValidationSetup>());
    }
}
