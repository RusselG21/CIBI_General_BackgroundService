namespace Shared.Abstraction;

public interface IWorkerConsumer<TIn, TOut>
{
    Task<int> HandleAsync(int page,CancellationToken cancellationToken);
}
