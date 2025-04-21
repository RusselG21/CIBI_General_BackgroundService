namespace Shared.Abstraction;

public interface IWorkerConsumer<T>
{
    Task HandleAsync(CancellationToken cancellationToken);
}
