using RapidCMS.ModelMaker.Enums;

namespace RapidCMS.ModelMaker.Extenstions
{
    public static class PublishStateExtensions
    {
        public static PublishState Modify(this PublishState state)
            => state switch
            {
                PublishState.Draft => PublishState.Draft,
                _ => PublishState.Changed
            };

        public static PublishState Publish(this PublishState state)
            => PublishState.Published;
    }
}
