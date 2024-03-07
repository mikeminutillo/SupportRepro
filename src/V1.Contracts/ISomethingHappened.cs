namespace Contracts
{
    using NServiceBus;

    #region V1Message

    public interface ISomethingHappened : IEvent
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

    public class SomethingMoreHappened : ISomethingHappened
    {
        public int SomeData { get; set; }
        public ComplexChild Child { get; set; } = new ComplexChild();

        IComplexChild ISomethingHappened.Child => Child;
    }


    #endregion
}
