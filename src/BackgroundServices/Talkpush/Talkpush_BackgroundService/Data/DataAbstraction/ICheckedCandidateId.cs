namespace Talkpush_BackgroundService.Data.DataAbstraction;

public interface ICheckedCandidateId
{
    Task<bool> CheckCandidateIdAsync(int candidateId, CancellationToken cancellationToken);
}
