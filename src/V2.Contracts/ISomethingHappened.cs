namespace Contracts
{

    public interface ISomethingHappened
    {
        int SomeData { get; set; }
        IComplexChild Child { get; }
    }

    public interface IComplexChild
    {
        bool IsComplex { get; }
    }

    public class ComplexChild : IComplexChild
    {
        public bool IsComplex => true;
    }
}
