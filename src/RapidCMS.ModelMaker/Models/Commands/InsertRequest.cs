namespace RapidCMS.ModelMaker.Models.Commands
{
    public class InsertRequest<TEntity>
    {
        public InsertRequest(TEntity entity)
        {
            Entity = entity;
        }

        public TEntity Entity { get; set; }
    }
}
