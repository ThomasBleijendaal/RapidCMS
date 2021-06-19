﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Repositories;
using RapidCMS.Example.ModelMaker.Components;
using RapidCMS.Example.ModelMaker.Validators;
using RapidCMS.Example.Shared.Collections;
using RapidCMS.Example.Shared.Data;
using RapidCMS.ModelMaker;
using RapidCMS.Repositories;

#nullable enable

//namespace RapidCMS.ModelMaker
//{
//    public class Blog2Repository : BaseRepository<Blog>
//    {
//        private readonly ModelMakerDbContext _dbContext;

//        public Blog2Repository(ModelMakerDbContext dbContext)
//        {
//            _dbContext = dbContext;
//        }

//        public override async Task DeleteAsync(string id, IParent? parent)
//        {
//            if (int.TryParse(id, out var intId))
//            {
//                var entity = await _dbContext.Blogs.Include(x => x.Categories).FirstOrDefaultAsync(x => x.Id == intId);
//                if (entity != null)
//                {
//                    _dbContext.Blogs.Remove(entity);
//                    await _dbContext.SaveChangesAsync();
//                }
//            }
//        }

//        public override async Task<IEnumerable<Blog>> GetAllAsync(IParent? parent, IQuery<Blog> query)
//        {
//            return await query.ApplyOrder(query.ApplyDataView(_dbContext.Blogs)).Skip(query.Skip).Take(query.Take).AsNoTracking().ToListAsync();
//        }

//        public override async Task<Blog?> GetByIdAsync(string id, IParent? parent)
//        {
//            if (int.TryParse(id, out var intId))
//            {
//                return await _dbContext.Blogs.Include(x => x.Categories).AsNoTracking().FirstOrDefaultAsync(x => x.Id == intId);
//            }
//            return default;
//        }

//        public override async Task<Blog?> InsertAsync(IEditContext<Blog> editContext)
//        {
//            var entity = editContext.Entity;

//            var relations = editContext.GetRelationContainer();
//            await HandleCategoriesAsync(entity, relations);

//            var entry = _dbContext.Blogs.Add(entity);
//            await _dbContext.SaveChangesAsync();
//            return entry.Entity;
//        }

//        public override Task<Blog> NewAsync(IParent? parent, Type? variantType = null)
//        {
//            return Task.FromResult(new Blog());
//        }

//        public override async Task UpdateAsync(IEditContext<Blog> editContext)
//        {
//            var entity = editContext.Entity;

//            var dbEntity = await _dbContext.Blogs.Include(x => x.Categories).FirstAsync(x => x.Id == entity.Id);

//            dbEntity.Content = entity.Content;
//            dbEntity.IsPublished = entity.IsPublished;
//            dbEntity.MainCategoryId = entity.MainCategoryId;
//            dbEntity.PublishDate = entity.PublishDate;
//            dbEntity.Title = entity.Title;

//            var relations = editContext.GetRelationContainer();
//            await HandleCategoriesAsync(dbEntity, relations);

//            await _dbContext.SaveChangesAsync();
//        }

//        private async Task HandleCategoriesAsync(Blog dbEntity, IRelationContainer relations)
//        {
//            var selectedIds = relations.GetRelatedElementIdsFor<Blog, ICollection<Category>, int>(x => x.Categories) ?? Enumerable.Empty<int>();
//            var existingIds = dbEntity.Categories.Select(x => x.Id);

//            var itemsToRemove = dbEntity.Categories.Where(x => !selectedIds.Contains(x.Id)).ToList();
//            var idsToAdd = selectedIds.Except(existingIds).ToList();

//            var itemsToAdd = await _dbContext.Categories.Where(x => idsToAdd.Contains(x.Id)).ToListAsync();

//            foreach (var itemToRemove in itemsToRemove)
//            {
//                dbEntity.Categories.Remove(itemToRemove);
//            }
//            foreach (var itemToAdd in itemsToAdd)
//            {
//                dbEntity.Categories.Add(itemToAdd);
//            }
//        }
//    }
//}


namespace RapidCMS.Example.ModelMaker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddModelMaker(
                // this flag adds the default properties and validators covering most of the use cases
                // each of the features added can also be added manually, to fully control the feature set of the model maker
                // (see ConfigurationExtensions.cs of RapidCMS.ModelMaker to see what is added when this flag is true)
                addDefaultPropertiesAndValidators: true,
                config =>
                {
                    // extra editors can be inserted easily
                    var customTextAreaEditor = config.AddPropertyEditor<CustomTextAreaEditor>("ctae", "Big textarea");

                    // adding new editors to existing properties is possible
                    config.GetProperty(Constants.Properties.ShortString)?.Editors.Add(customTextAreaEditor);

                    // custom editor validators can be inserted easily
                    var customTextValidator = config.AddPropertyValidator<string, BannedContentValidationConfig, List<string>>(
                        "bannedContent",
                        "Banned Content",
                        "The content is not allowed contain the following words.",
                        EditorType.ListEditor,
                        property => property.Config.BannedWords);

                    // adding extra validations to existing properties is possible
                    config.GetProperty(Constants.Properties.ShortString)?.Validators.Add(customTextValidator);
                    config.GetProperty(Constants.Properties.LongString)?.Validators.Add(customTextValidator);

                    // adding custom properties is also possible
                    //var enumDropdownValidator = config.AddPropertyValidator<ContentType, NoConfig, NoConfig, EnumOptionsDataCollectionFactory<ContentType>>(
                    //    "contentTypeValidator",
                    //    "Content Type",
                    //    "Content Type",
                    //    EditorType.None,
                    //    property => property.Config);

                    // TODO: enums gets eaten up by the json serializer
                    //config.AddProperty<ContentType>(
                    //    "contentType",
                    //    "Content Type",
                    //    "Tag",
                    //    new[] { Constants.Editors.Dropdown, Constants.Editors.Select },
                    //    new[] { enumDropdownValidator.Alias });
                });

            // validators are resolved from DI
            //services.AddSingleton<BannedContentValidator>();
            //services.AddSingleton<EnumOptionsValidator<ContentType>>();

            // factories are also resolved from DI
            // services.AddSingleton<EnumOptionsDataCollectionFactory<ContentType>>();

            services.AddScoped<BaseRepository<Person>, JsonRepository<Person>>();
            services.AddScoped<BaseRepository<Details>, JsonRepository<Details>>();

            services.AddScoped<BaseRepository<Blog>, BlogRepository>();
            services.AddScoped<BaseRepository<Category>, CategoryRepository>();

            services.AddDbContext<ModelMakerDbContext>(
                builder => builder.UseSqlServer(Configuration.GetConnectionString("SqlConnectionString")),
                ServiceLifetime.Transient,
                ServiceLifetime.Transient);

            services.AddRapidCMSServer(config =>
            {
                config.AllowAnonymousUser();

                config.SetSiteName("Model maker");

                config.AddBlogCollection();
                config.AddCategoryCollection();

                config.AddPersonCollection();

                config.AddModelMakerPlugin();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ModelMakerDbContext context)
        {
            context.Database.Migrate();

            app.UseRapidCMS(isDevelopment: env.IsDevelopment());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
