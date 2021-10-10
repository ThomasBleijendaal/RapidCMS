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

            foreach (var relatedProperty in info.Properties.Where(x => x.Relation.HasFlag(Relation.ToMany) && !x.Hidden))
            {
                WriteHandleRelationMethod(indentWriter, context, info, relatedProperty);
            }

            WriteClosingBracket(indentWriter);

            WriteClosingBracket(indentWriter);

            return SourceText.From(writer.ToString(), Encoding.UTF8);
        }

        public void WriteRepository(IndentedTextWriter indentWriter, EntityInformation info)
        {
            indentWriter.WriteLine($"public class {info.PascalCaseName}Repository : BaseRepository<{info.PascalCaseName}>");
            WriteOpeningBracket(indentWriter);

            indentWriter.WriteLine("private readonly ModelMakerDbContext _dbContext;");
            indentWriter.WriteLine();

            indentWriter.WriteLine($"public {info.PascalCaseName}Repository(ModelMakerDbContext dbContext)");
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
            indentWriter.WriteLine($"var entity = await _dbContext.{info.PascalCasePluralName}{GetIncludes(info)}.FirstOrDefaultAsync(x => x.Id == intId);");
            indentWriter.WriteLine("if (entity != null)");
            WriteOpeningBracket(indentWriter);
            indentWriter.WriteLine($"_dbContext.{info.PascalCasePluralName}.Remove(entity);");
            indentWriter.WriteLine("await _dbContext.SaveChangesAsync();");
            WriteClosingBracket(indentWriter);
            WriteClosingBracket(indentWriter);
            WriteClosingBracket(indentWriter);
        }

        public void WriteGetAllAsync(IndentedTextWriter indentWriter, EntityInformation info)
        {
            indentWriter.WriteLine();
            indentWriter.WriteLine($"public override async Task<IEnumerable<{info.PascalCaseName}>> GetAllAsync(IParent? parent, IView<{info.PascalCaseName}> view)");
            WriteOpeningBracket(indentWriter);
            indentWriter.Write($"return await view.ApplyOrder(view.ApplyDataView(_dbContext.{info.PascalCasePluralName}))");
            indentWriter.Write(".Skip(view.Skip)");
            indentWriter.Write(".Take(view.Take)");
            indentWriter.Write(".AsNoTracking()");
            indentWriter.WriteLine(".ToListAsync();");
            WriteClosingBracket(indentWriter);
        }

        public void GetByIdAsync(IndentedTextWriter indentWriter, EntityInformation info)
        {
            indentWriter.WriteLine();
            indentWriter.WriteLine($"public override async Task<{info.PascalCaseName}?> GetByIdAsync(string id, IParent? parent)");
            WriteOpeningBracket(indentWriter);
            indentWriter.WriteLine("if (int.TryParse(id, out var intId))");
            WriteOpeningBracket(indentWriter);
            indentWriter.WriteLine($"return await _dbContext.{info.PascalCasePluralName}{GetIncludesAndAsNoTracking(info)}.FirstOrDefaultAsync(x => x.Id == intId);");
            WriteClosingBracket(indentWriter);
            indentWriter.WriteLine("return default;");
            WriteClosingBracket(indentWriter);
        }

        public void WriteInsertAsync(IndentedTextWriter indentWriter, EntityInformation info)
        {
            indentWriter.WriteLine();
            indentWriter.WriteLine($"public override async Task<{info.PascalCaseName}?> InsertAsync(IEditContext<{info.PascalCaseName}> editContext)");
            WriteOpeningBracket(indentWriter);
            indentWriter.WriteLine("var entity = editContext.Entity;");
            WriteHandleRelations(indentWriter, info);
            indentWriter.WriteLine();
            indentWriter.WriteLine($"var entry = _dbContext.{info.PascalCasePluralName}.Add(entity);");
            indentWriter.WriteLine("await _dbContext.SaveChangesAsync();");
            indentWriter.WriteLine("return entry.Entity;");
            WriteClosingBracket(indentWriter);
        }

        public void WriteNewAsync(IndentedTextWriter indentWriter, EntityInformation info)
        {
            indentWriter.WriteLine();
            indentWriter.WriteLine($"public override Task<{info.PascalCaseName}> NewAsync(IParent? parent, Type? variantType = null)");
            WriteOpeningBracket(indentWriter);
            indentWriter.WriteLine($"return Task.FromResult(new {info.PascalCaseName}());");
            WriteClosingBracket(indentWriter);
        }

        public void WriteUpdateAsync(IndentedTextWriter indentWriter, EntityInformation info)
        {
            indentWriter.WriteLine();
            indentWriter.WriteLine($"public override async Task UpdateAsync(IEditContext<{info.PascalCaseName}> editContext)");
            WriteOpeningBracket(indentWriter);
            indentWriter.WriteLine($"var entity = await _dbContext.{info.PascalCasePluralName}{GetIncludes(info)}.FirstAsync(x => x.Id == editContext.Entity.Id);");
            WritePropertyMap(indentWriter, info);
            WriteHandleRelations(indentWriter, info);
            indentWriter.WriteLine();
            indentWriter.WriteLine("await _dbContext.SaveChangesAsync();");
            WriteClosingBracket(indentWriter);
        }

        private void WritePropertyMap(IndentedTextWriter indentWriter, EntityInformation info)
        {
            indentWriter.WriteLine();
            foreach (var property in info.Properties.Where(x => !x.Relation.HasFlag(Relation.ToMany) && !x.Hidden).OrderBy(x => x.Name))
            {
                if (property.Relation.HasFlag(Relation.One | Relation.ToOne) && !property.Relation.HasFlag(Relation.DependentSide))
                {
                    // disabled editor
                }
                else if (property.Relation.HasFlag(Relation.ToOne))
                {
                    indentWriter.WriteLine($"entity.{property.PascalCaseName}Id = editContext.Entity.{property.PascalCaseName}Id;");
                }
                else
                { 
                    indentWriter.WriteLine($"entity.{property.PascalCaseName} = editContext.Entity.{property.PascalCaseName};");
                }
            }
        }

        private void WriteHandleRelations(IndentedTextWriter indentWriter, EntityInformation info)
        {
            var relatedProperties = info.Properties.Where(x => x.Relation.HasFlag(Relation.ToMany) && !x.Hidden);

            if (!relatedProperties.Any())
            {
                return;
            }

            indentWriter.WriteLine();
            indentWriter.WriteLine("var relations = editContext.GetRelationContainer();");
            foreach (var relatedProperty in relatedProperties)
            {
                indentWriter.WriteLine($"await Handle{relatedProperty.PascalCaseName}Async(entity, relations);");
            }
        }

        private void WriteHandleRelationMethod(IndentedTextWriter indentWriter, ModelMakerContext context, EntityInformation entity, PropertyInformation property)
        {
            var relatedDbContextName = context.Entities.FirstOrDefault(x => x.Alias == property.RelatedCollectionAlias)?.PascalCasePluralName;

            if (string.IsNullOrEmpty(relatedDbContextName))
            {
                indentWriter.WriteLine();
                indentWriter.WriteLine($"protected virtual Task Handle{property.PascalCaseName}Async({entity.PascalCaseName} dbEntity, IRelationContainer relations)");
                WriteOpeningBracket(indentWriter);
                indentWriter.WriteLine("return Task.CompletedTask;");
                WriteClosingBracket(indentWriter);
            }
            else
            {
                indentWriter.WriteLine();
                indentWriter.WriteLine($"private async Task Handle{property.PascalCaseName}Async({entity.PascalCaseName} dbEntity, IRelationContainer relations)");
                WriteOpeningBracket(indentWriter);
                indentWriter.WriteLine($"var selectedIds = relations.GetRelatedElementIdsFor<{entity.PascalCaseName}, ICollection<{property.Type}>, int>(x => x.{property.PascalCaseName}) ?? Enumerable.Empty<int>();");
                indentWriter.WriteLine($"var existingIds = dbEntity.{property.PascalCaseName}.Select(x => x.Id);");
                indentWriter.WriteLine();
                indentWriter.WriteLine($"var itemsToRemove = dbEntity.{property.PascalCaseName}.Where(x => !selectedIds.Contains(x.Id)).ToList();");
                indentWriter.WriteLine("var idsToAdd = selectedIds.Except(existingIds).ToList();");
                indentWriter.WriteLine();
                indentWriter.WriteLine($"var itemsToAdd = await _dbContext.{relatedDbContextName}.Where(x => idsToAdd.Contains(x.Id)).ToListAsync();");
                indentWriter.WriteLine();
                indentWriter.WriteLine($"foreach (var itemToRemove in itemsToRemove)");
                WriteOpeningBracket(indentWriter);
                indentWriter.WriteLine($"dbEntity.{property.PascalCaseName}.Remove(itemToRemove);");
                WriteClosingBracket(indentWriter);
                indentWriter.WriteLine($"foreach (var itemToAdd in itemsToAdd)");
                WriteOpeningBracket(indentWriter);
                indentWriter.WriteLine($"dbEntity.{property.PascalCaseName}.Add(itemToAdd);");
                WriteClosingBracket(indentWriter);
                WriteClosingBracket(indentWriter);
            }
        }

        private string GetIncludesAndAsNoTracking(EntityInformation info)
            => $"{string.Join("", GetAllIncludes(info))}.AsNoTracking()";
        private string GetIncludes(EntityInformation info)
            => $"{string.Join("", GetAllIncludes(info))}";

        private IEnumerable<string> GetAllIncludes(EntityInformation info)
        {
            foreach (var property in info.Properties.Where(RelatedEntities))
            {
                yield return $".Include(x => x.{property.PascalCaseName})";
            }
        }

        private static bool RelatedEntities(PropertyInformation x)
        {
            return (x.Relation.HasFlag(Relation.ToMany) && !x.Hidden) ||
                (x.Relation.HasFlag(Relation.One | Relation.ToOne) && !x.Relation.HasFlag(Relation.DependentSide));
        }
    }
}
