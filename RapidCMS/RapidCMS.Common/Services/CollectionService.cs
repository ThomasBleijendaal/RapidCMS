using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.DTOs;

namespace RapidCMS.Common.Services
{
    public interface ICollectionService
    {
        IEnumerable<CollectionTreeDTO> GetCollections();
    }

    public class CollectionService : ICollectionService
    {
        private readonly Root _root;

        public CollectionService(Root root)
        {
            _root = root;
        }
        
        public IEnumerable<CollectionTreeDTO> GetCollections()
        {
            foreach (var collection in _root.Collections)
            {
                var entities = collection.Repository.GetAllAsObjects();

                yield return new CollectionTreeDTO
                {
                    Name = collection.Name,
                    Entities = entities.Select(obj => new CollectionTreeEntity
                    {
                        Id = obj.Id,
                        Name = collection.TreeView.NameGetter.Invoke(obj) as string
                    }).ToList()
                };
            }
        }
    }
}
