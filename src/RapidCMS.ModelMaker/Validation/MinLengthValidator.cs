//using RapidCMS.Core.Abstractions.Data;
//using RapidCMS.Core.Validators;

//namespace RapidCMS.ModelMaker.Validation
//{
//    public class MinLengthValidator : BaseEntityValidator<IEntity>
//    {

//    }

//    public static class MyCustomValidators
//    {
//        public static IRuleBuilderOptions<T, IList<TElement>> ListMustContainFewerThan<T, TElement>(this IRuleBuilder<T, IList<TElement>> ruleBuilder, int num)
//        {
//            return ruleBuilder.Must(list => list.Count < num).WithMessage("The list contains too many items");
//        }
//    }
//}
