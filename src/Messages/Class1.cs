using MessageContracts;

namespace Messages;

public class ConcreteContract : IComplexContract
{
    public ConcreteChild? Child { get; set; }
    IComplexChild? IComplexContract.Child => Child;
}

public class ConcreteChild : IComplexChild
{
    public bool IsComplex => true;
}