using COMP1640_WebDev.Models;

namespace COMP1640_WebDev.Repositories.Interfaces
{
    public interface IContributionRepository
    {
        Task<IEnumerable<Contribution>> GetContributions();
        Task<IEnumerable<Contribution>> GetContributionsInprogess();

        Task<List<Contribution>> GetContributionsAccept();
        Task<Contribution> GetContribution(string idContribution);
        Task<Contribution> CreateContribution(Contribution contribution, IFormFile? formFile);
        Task<Contribution> RemoveContribution(string idContribution);
        Task<Contribution> UpdateContribution(string idContribution, Contribution contribution);
        Task<int> CountFiles();
        Task<int> CountAcceptedFiles();
        Task<int> CountRejectedFiles();
        Task<int> CountInprogressFiles();
    }
}
