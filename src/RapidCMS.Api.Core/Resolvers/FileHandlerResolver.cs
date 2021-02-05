using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RapidCMS.Api.Core.Abstractions;

namespace RapidCMS.Api.Core.Resolvers
{
    internal class FileHandlerResolver : IFileHandlerResolver
    {
        public FileHandlerResolver()
        {
        }

        public IFileHandler GetApiHandler(string uploadHandlerAlias)
        {
            throw new NotImplementedException();
        }
    }
}
