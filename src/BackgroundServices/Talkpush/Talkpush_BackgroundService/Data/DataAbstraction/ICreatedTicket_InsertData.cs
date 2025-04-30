namespace Talkpush_BackgroundService.Data.DataAbstraction;

public interface ICreatedTicket_InsertData
{
    Task<bool> InsertDataAsync(
        CreatedTicket createdTicket,
        CancellationToken cancellationToken);
}
