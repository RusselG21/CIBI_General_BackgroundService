namespace Shared.Abstraction;

public interface IWorkerConsumer<TIn, TOut>
{
    Task HandleAsync(CancellationToken cancellationToken);
}
