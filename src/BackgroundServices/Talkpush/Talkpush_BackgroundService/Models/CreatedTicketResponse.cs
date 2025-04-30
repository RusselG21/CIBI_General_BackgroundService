namespace Talkpush_BackgroundService.Models;


public class CreatedTicketResponse
{
    public string? TicketNumber { get; set; }
    public string? DeliveryDate { get; set; }
    public string ResultCode { get; set; } = default!;
    public string ResultMessage { get; set; } = default!;
}