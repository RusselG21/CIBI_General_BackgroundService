namespace Shared.Abstraction;

public interface ITransformer<TIn, TOut>
{
    TOut Transform(TIn input);
}
