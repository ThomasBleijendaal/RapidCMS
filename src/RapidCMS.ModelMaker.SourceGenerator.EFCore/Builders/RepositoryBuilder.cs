using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.Text;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Contexts;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Enums;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Information;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Builders
{
    internal sealed class RepositoryBuilder : BuilderBase
    {
        public SourceText? BuildRepository(EntityInformation info, ModelMakerContext context)
        {
            if (!info.OutputItems.Contains(Constants.OutputRepository))
            {
                return default;
            }

            using var writer = new StringWriter();
            using var indentWriter = new IndentedTextWriter(writer, "    ");

            var namespaces = context.Entities.SelectMany(x => x.NamespacesUsed(Use.Repository));
            WriteUsingNamespaces(indentWriter, namespaces);
            WriteOpenNamespace(indentWriter, context.Namespace);

            WriteRepository(indentWriter, info);

            WriteDeleteAsync(indentWriter, info);
            WriteGetAllAsync(indentWriter, info);
            GetByIdAsync(indentWriter, info);
            WriteInsertAsync(indentWriter, info);
            WriteNewAsync(indentWriter, info);
            WriteUpdateAsync(indentWriter, info);

            WriteClosingBracket(indentWriter);

            WriteClosingBracket(indentWriter);

            return SourceText.From(writer.ToString(), Encoding.UTF8);
        }

        public void WriteRepository(IndentedTextWriter indentWriter, EntityInformation info)
        {
            indentWriter.WriteLine($"public class {info.Name}Repository : BaseRepository<{info.Name}>");
            WriteOpeningBracket(indentWriter);

            indentWriter.WriteLine("private readonly ModelMakerDbContext _dbContext;");
            indentWriter.WriteLine();

            indentWriter.WriteLine($"public {info.Name}Repository(ModelMakerDbContext dbContext)");
            WriteOpeningBracket(indentWriter);
            indentWriter.WriteLine("_dbContext = dbContext;");
            WriteClosingBracket(indentWriter);
        }

        public void WriteDeleteAsync(IndentedTextWriter indentWriter, EntityInformation info)
        {
            indentWriter.WriteLine();
            indentWriter.WriteLine("public override async Task DeleteAsync(string id, IParent? parent)");
            WriteOpeningBracket(indentWriter);
            indentWriter.WriteLine("var entity = await GetByIdAsync(id, parent);");
            indentWriter.WriteLine("if (entity != null)");
            WriteOpeningBracket(indentWriter);
            indentWriter.WriteLine($"_dbContext.{info.PluralName}.Remove(entity);");
            indentWriter.WriteLine("await _dbContext.SaveChangesAsync();");
            WriteClosingBracket(indentWriter);
            WriteClosingBracket(indentWriter);
        }

        public void WriteGetAllAsync(IndentedTextWriter indentWriter, EntityInformation info)
        {
            indentWriter.WriteLine();
            indentWriter.WriteLine($"public override async Task<IEnumerable<{info.Name}>> GetAllAsync(IParent? parent, IQuery<{info.Name}> query)");
            WriteOpeningBracket(indentWriter);
            indentWriter.WriteLine($"return await query.ApplyOrder(query.ApplyDataView(_dbContext.{info.PluralName}))");
            indentWriter.Indent++;
            indentWriter.WriteLine(".Skip(query.Skip)");
            indentWriter.WriteLine(".Take(query.Take)");
            indentWriter.WriteLine(".ToListAsync();");
            indentWriter.Indent--;
            WriteClosingBracket(indentWriter);
        }

        public void GetByIdAsync(IndentedTextWriter indentWriter, EntityInformation info)
        {
            indentWriter.WriteLine();
            indentWriter.WriteLine($"public override async Task<{info.Name}?> GetByIdAsync(string id, IParent? parent)");
            WriteOpeningBracket(indentWriter);
            indentWriter.WriteLine("if (int.TryParse(id, out var intId))");
            WriteOpeningBracket(indentWriter);
            indentWriter.WriteLine($"return await _dbContext.{info.PluralName}.FirstOrDefaultAsync(x => x.Id == intId);");
            WriteClosingBracket(indentWriter);
            indentWriter.WriteLine("return default;");
            WriteClosingBracket(indentWriter);
        }

        public void WriteInsertAsync(IndentedTextWriter indentWriter, EntityInformation info)
        {
            indentWriter.WriteLine();
            indentWriter.WriteLine($"public override async Task<{info.Name}?> InsertAsync(IEditContext<{info.Name}> editContext)");
            WriteOpeningBracket(indentWriter);
            indentWriter.WriteLine($"var entry = _dbContext.{info.PluralName}.Add(editContext.Entity);");
            indentWriter.WriteLine("await _dbContext.SaveChangesAsync();");
            indentWriter.WriteLine("return entry.Entity;");
            WriteClosingBracket(indentWriter);
        }

        public void WriteNewAsync(IndentedTextWriter indentWriter, EntityInformation info)
        {
            indentWriter.WriteLine();
            indentWriter.WriteLine($"public override Task<{info.Name}> NewAsync(IParent? parent, Type? variantType = null)");
            WriteOpeningBracket(indentWriter);
            indentWriter.WriteLine($"return Task.FromResult(new {info.Name}());");
            WriteClosingBracket(indentWriter);
        }

        public void WriteUpdateAsync(IndentedTextWriter indentWriter, EntityInformation info)
        {
            indentWriter.WriteLine();
            indentWriter.WriteLine($"public override async Task UpdateAsync(IEditContext<{info.Name}> editContext)");
            WriteOpeningBracket(indentWriter);
            indentWriter.WriteLine("await _dbContext.SaveChangesAsync();");
            WriteClosingBracket(indentWriter);
        }
    }
}
