using COMP1640_WebDev.Data;
using COMP1640_WebDev.Models;
using COMP1640_WebDev.Repositories.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;

namespace COMP1640_WebDev.Repositories
{
	public class ContributionRepository : IContributionRepository
	{
		private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public ContributionRepository(ApplicationDbContext dbContext, UserManager<User> userManager)
		{
			_dbContext = dbContext;
			_userManager = userManager;
		}
		public async Task<Contribution> CreateContribution(Contribution contribution, IFormFile? formFile)
		{
			using (var memoryStream = new MemoryStream())
			{
				await formFile!.CopyToAsync(memoryStream);

				contribution.Image = memoryStream.ToArray();


				var result = await _dbContext.Contributions.AddAsync(contribution);
				await _dbContext.SaveChangesAsync();

				return result.Entity;
			}
		}

		public async Task<Contribution> GetContribution(string idContribution)
		{
			var contributionInDB = _dbContext.Contributions
			.Include(i => i.AcademicYear)
			.Include(u => u.User)
			.SingleOrDefault(i => i.Id == idContribution);

			if (contributionInDB == null)
			{
				return null;
			}

			return contributionInDB;
		}

		public async Task<IEnumerable<Contribution>> GetContributions()
		{
			return await _dbContext.Contributions.ToListAsync();

		}

		public async Task<IEnumerable<Contribution>> GetContributionsInprogess(string facID)
		{

            return await _dbContext.Contributions.Include(u => u.User).Where(c => c.Status == Enum.BrowserComment.InProgess && c.User.FacultyId == facID).ToListAsync();

		}

		public async Task<List<Contribution>> GetContributionsAccept(string magazineID)
		{
			return await _dbContext.Contributions.Include(u => u.User).Where(c => c.Status == Enum.BrowserComment.Accepted && c.MagazineId == magazineID).ToListAsync();

		}
		public Task<Contribution> RemoveContribution(string idContribution)
		{
			throw new NotImplementedException();
		}


		public async Task<Contribution> UpdateContribution(string idContribution, Contribution contribution)
		{
			var academicYear = await _dbContext.AcademicYears.SingleOrDefaultAsync(a => a.Id == contribution.AcademicYearId);

			if (contribution.SubmissionDate > academicYear.FinalDate)
			{
				return null;
			}
			else
			{
				var contributionInDb = await _dbContext.Contributions
			   .SingleOrDefaultAsync(e => e.Id == idContribution);

				if (contributionInDb == null)
				{
					return null;
				}

				contributionInDb.Title = contribution.Title;
				contributionInDb.Document = contribution.Document;
				contributionInDb.Image = contribution.Image;
				await _dbContext.SaveChangesAsync();

				return contribution;
			}
		}

		public Task<Contribution> CreateContribution(Contribution contribution)
		{
			throw new NotImplementedException();
		}
	}
}