using COMP1640_WebDev.Models;

namespace COMP1640_WebDev.Repositories.Interfaces
{
    public interface IContributionRepository
    {
        Task<IEnumerable<Contribution>> GetContributions();
        Task<IEnumerable<Contribution>> GetContributionsInprogess(string facID);

        Task<List<Contribution>> GetContributionsAccept(string magazineID);
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
