namespace RapidCMS.ModelMaker.Models.Commands
{
    public class UpdateRequest<TEntity>
    {
        public UpdateRequest(TEntity entity)
        {
            Entity = entity;
        }

        public TEntity Entity { get; set; }
    }
}
