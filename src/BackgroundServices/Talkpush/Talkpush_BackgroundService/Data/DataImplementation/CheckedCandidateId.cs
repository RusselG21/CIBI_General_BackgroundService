using Talkpush_BackgroundService.Data.DataAbstraction;

namespace Talkpush_BackgroundService.Data.DataImplementation;

public class CheckedCandidateId : ICheckedCandidateId
{
    private readonly TalkpushDBContext _dBContext;

    public CheckedCandidateId(TalkpushDBContext dBContext)
    {
        this._dBContext = dBContext;
    }

    public async Task<bool> CheckCandidateIdAsync(int candidateId, CancellationToken cancellationToken)
    {
       var isCandidateIdExists = await _dBContext.CreatedTickets
            .AnyAsync(c => c.Candidate_Primary_Id == candidateId, cancellationToken);
        return isCandidateIdExists;
    }
}
