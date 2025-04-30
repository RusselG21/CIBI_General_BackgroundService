namespace Talkpush_BackgroundService.Data;

public class TalkpushDBContext : DbContext
{
    public TalkpushDBContext(DbContextOptions<TalkpushDBContext> options) : base(options)
    {
    }

   public DbSet<CreatedTicket> CreatedTickets { get; set; }

}
