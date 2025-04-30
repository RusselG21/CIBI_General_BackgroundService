using Talkpush_BackgroundService.Data.DataAbstraction;

namespace Talkpush_BackgroundService.Data.DataImplementation;

public class CreatedTicket_InsertData : ICreatedTicket_InsertData
{
    private readonly TalkpushDBContext dbContext;

    public CreatedTicket_InsertData(TalkpushDBContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task<bool> InsertDataAsync(CreatedTicket createdTicket, CancellationToken cancellationToken)
    {
        await dbContext.CreatedTickets.AddAsync(createdTicket, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
