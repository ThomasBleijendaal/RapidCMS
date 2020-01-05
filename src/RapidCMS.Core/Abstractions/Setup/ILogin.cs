namespace RapidCMS.Core.Abstractions.Setup
{
    public interface ILogin
    {
        ITypeRegistration? CustomLoginScreenRegistration { get; }
        ITypeRegistration? CustomLoginStatusRegistration { get; }
    }
}
