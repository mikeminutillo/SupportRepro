namespace MessageContracts;

public interface IComplexContract
{
    IComplexChild? Child { get; }
}

public interface IComplexChild
{
    bool IsComplex { get; }
}
