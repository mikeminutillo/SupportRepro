namespace Contracts
{
    #region V2Message

    public interface ISomethingMoreHappened : ISomethingHappened
    {
        string MoreInfo { get; set; }
    }

    public class SomethingMoreHappened : ISomethingMoreHappened
    {
        public int SomeData { get; set; }
        public ComplexChild Child { get; set; } = new ComplexChild();
        public string MoreInfo { get; set; }


        IComplexChild ISomethingHappened.Child => Child;
    }

    #endregion
}
