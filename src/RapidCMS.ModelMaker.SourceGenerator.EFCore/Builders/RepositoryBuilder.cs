using System.CodeDom.Compiler;
using System.Collections.Generic;
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

            foreach (var relatedProperty in info.Properties.Where(x => x.RelatedToManyEntities && !x.Hidden))
            {
                WriteHandleRelationMethod(indentWriter, info, relatedProperty);
            }

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
            indentWriter.WriteLine("if (int.TryParse(id, out var intId))");
            WriteOpeningBracket(indentWriter);
            indentWriter.WriteLine($"var entity = await _dbContext.{info.PluralName}{GetIncludes(info)}.FirstOrDefaultAsync(x => x.Id == intId);");
            indentWriter.WriteLine("if (entity != null)");
            WriteOpeningBracket(indentWriter);
            indentWriter.WriteLine($"_dbContext.{info.PluralName}.Remove(entity);");
            indentWriter.WriteLine("await _dbContext.SaveChangesAsync();");
            WriteClosingBracket(indentWriter);
            WriteClosingBracket(indentWriter);
            WriteClosingBracket(indentWriter);
        }

        public void WriteGetAllAsync(IndentedTextWriter indentWriter, EntityInformation info)
        {
            indentWriter.WriteLine();
            indentWriter.WriteLine($"public override async Task<IEnumerable<{info.Name}>> GetAllAsync(IParent? parent, IQuery<{info.Name}> query)");
            WriteOpeningBracket(indentWriter);
            indentWriter.Write($"return await query.ApplyOrder(query.ApplyDataView(_dbContext.{info.PluralName}))");
            indentWriter.Write(".Skip(query.Skip)");
            indentWriter.Write(".Take(query.Take)");
            indentWriter.Write(".AsNoTracking()");
            indentWriter.WriteLine(".ToListAsync();");
            WriteClosingBracket(indentWriter);
        }

        public void GetByIdAsync(IndentedTextWriter indentWriter, EntityInformation info)
        {
            indentWriter.WriteLine();
            indentWriter.WriteLine($"public override async Task<{info.Name}?> GetByIdAsync(string id, IParent? parent)");
            WriteOpeningBracket(indentWriter);
            indentWriter.WriteLine("if (int.TryParse(id, out var intId))");
            WriteOpeningBracket(indentWriter);
            indentWriter.WriteLine($"return await _dbContext.{info.PluralName}{GetIncludesAndAsNoTracking(info)}.FirstOrDefaultAsync(x => x.Id == intId);");
            WriteClosingBracket(indentWriter);
            indentWriter.WriteLine("return default;");
            WriteClosingBracket(indentWriter);
        }

        public void WriteInsertAsync(IndentedTextWriter indentWriter, EntityInformation info)
        {
            indentWriter.WriteLine();
            indentWriter.WriteLine($"public override async Task<{info.Name}?> InsertAsync(IEditContext<{info.Name}> editContext)");
            WriteOpeningBracket(indentWriter);
            indentWriter.WriteLine("var entity = editContext.Entity;");
            WriteHandleRelations(indentWriter, info);
            indentWriter.WriteLine();
            indentWriter.WriteLine($"var entry = _dbContext.{info.PluralName}.Add(entity);");
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
            indentWriter.WriteLine($"var entity = await _dbContext.{info.PluralName}{GetIncludes(info)}.FirstAsync(x => x.Id == editContext.Entity.Id);");
            WritePropertyMap(indentWriter, info);
            WriteHandleRelations(indentWriter, info);
            indentWriter.WriteLine();
            indentWriter.WriteLine("await _dbContext.SaveChangesAsync();");
            WriteClosingBracket(indentWriter);
        }

        private void WritePropertyMap(IndentedTextWriter indentWriter, EntityInformation info)
        {
            indentWriter.WriteLine();
            foreach (var property in info.Properties.Where(x => !x.RelatedToManyEntities && !x.Hidden).OrderBy(x => x.Name))
            {
                if (property.RelatedToOneEntity)
                {
                    indentWriter.WriteLine($"entity.{ValidPascalCaseName(property.Name)}Id = editContext.Entity.{ValidPascalCaseName(property.Name)}Id;");
                }
                else
                { 
                    indentWriter.WriteLine($"entity.{ValidPascalCaseName(property.Name)} = editContext.Entity.{ValidPascalCaseName(property.Name)};");
                }
            }
        }

        private void WriteHandleRelations(IndentedTextWriter indentWriter, EntityInformation info)
        {
            var relatedProperties = info.Properties.Where(x => x.RelatedToManyEntities && !x.Hidden);

            if (!relatedProperties.Any())
            {
                return;
            }

            indentWriter.WriteLine();
            indentWriter.WriteLine("var relations = editContext.GetRelationContainer();");
            foreach (var relatedProperty in relatedProperties)
            {
                indentWriter.WriteLine($"await Handle{ValidPascalCaseName(relatedProperty.Name)}Async(entity, relations);");
            }
        }

        private void WriteHandleRelationMethod(IndentedTextWriter indentWriter, EntityInformation entity, PropertyInformation property)
        {
            indentWriter.WriteLine();
            indentWriter.WriteLine($"private async Task Handle{ValidPascalCaseName(property.Name)}Async(Blog dbEntity, IRelationContainer relations)");
            WriteOpeningBracket(indentWriter);
            indentWriter.WriteLine($"var selectedIds = relations.GetRelatedElementIdsFor<Blog, ICollection<{property.Type}>, int>(x => x.{ValidPascalCaseName(property.Name)}) ?? Enumerable.Empty<int>();");
            indentWriter.WriteLine($"var existingIds = dbEntity.{ValidPascalCaseName(property.Name)}.Select(x => x.Id);");
            indentWriter.WriteLine();
            indentWriter.WriteLine($"var itemsToRemove = dbEntity.{ValidPascalCaseName(property.Name)}.Where(x => !selectedIds.Contains(x.Id)).ToList();");
            indentWriter.WriteLine("var idsToAdd = selectedIds.Except(existingIds).ToList();");
            indentWriter.WriteLine();
            indentWriter.WriteLine($"var itemsToAdd = await _dbContext.{ValidPascalCaseName(property.Name)}.Where(x => idsToAdd.Contains(x.Id)).ToListAsync();");
            indentWriter.WriteLine();
            indentWriter.WriteLine($"foreach (var itemToRemove in itemsToRemove)");
            WriteOpeningBracket(indentWriter);
            indentWriter.WriteLine($"dbEntity.{ValidPascalCaseName(property.Name)}.Remove(itemToRemove);");
            WriteClosingBracket(indentWriter);
            indentWriter.WriteLine($"foreach (var itemToAdd in itemsToAdd)");
            WriteOpeningBracket(indentWriter);
            indentWriter.WriteLine($"dbEntity.{ValidPascalCaseName(property.Name)}.Add(itemToAdd);");
            WriteClosingBracket(indentWriter);
            WriteClosingBracket(indentWriter);
        }

        private string GetIncludesAndAsNoTracking(EntityInformation info)
            => $"{string.Join("", GetAllIncludes(info))}.AsNoTracking()";
        private string GetIncludes(EntityInformation info)
            => $"{string.Join("", GetAllIncludes(info))}";

        private IEnumerable<string> GetAllIncludes(EntityInformation info)
        {
            foreach (var property in info.Properties.Where(x => x.RelatedToManyEntities && !x.Hidden))
            {
                yield return $".Include(x => x.{ValidPascalCaseName(property.Name)})";
            }
        }
    }
}
