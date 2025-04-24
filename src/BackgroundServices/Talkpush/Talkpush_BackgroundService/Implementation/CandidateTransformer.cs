namespace Talkpush_BackgroundService.Implementation;
public class CandidateTransformer : ITransformer<Candidate, CreateTicketCandidateRecord>
{
    public CreateTicketCandidateRecord Transform(Candidate candidate)
    {
        return candidate.Adapt<CreateTicketCandidateRecord>();
    }
}
