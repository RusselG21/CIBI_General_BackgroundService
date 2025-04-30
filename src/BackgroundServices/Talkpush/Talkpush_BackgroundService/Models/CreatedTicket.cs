namespace Talkpush_BackgroundService.Models;

public class CreatedTicket
{
    public Guid Id { get; set; }

    public int Candidate_Primary_Id { get; set; }

    public int? Candidate_Id { get; set; }

    public string? TicketNumber { get; set; }

    public DateTime TicketCreated { get; set; }
}
