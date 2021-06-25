using FluentValidation;
using RapidCMS.ModelMaker.Validation;

namespace RapidCMS.ModelMaker
{
    public class BlogValidator : AbstractValidatorAdapter<Blog>
    {
        public BlogValidator()
        {
            RuleFor(x => x.Title).MinimumLength(1);
        }
    }
}
