using COMP1640_WebDev.Data;
using COMP1640_WebDev.Models;
using COMP1640_WebDev.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using COMP1640_WebDev.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace COMP1640_WebDev.Controllers
{
	public class MarketingCoordinatorController(IContributionRepository contributionRepository, IEmailService emailService, UserManager<User> userManager) : Controller
	{
		private readonly IContributionRepository _contributionRepository = contributionRepository;
		private readonly IEmailService _emailService = emailService;
        private readonly UserManager<User> _userManager = userManager;


		[HttpGet]
		public async Task<IActionResult> IndexAsync()

		{
            int acceptedFilesData = await _contributionRepository.CountAcceptedFiles();
            int rejectedFilesData = await _contributionRepository.CountRejectedFiles();
            int inprogressFileData = await _contributionRepository.CountInprogressFiles();

            ViewBag.AcceptedData = acceptedFilesData;
            ViewBag.RejectedData = rejectedFilesData;
            ViewBag.WaitingData = inprogressFileData;

            return View();
		}

		[HttpGet]
		public async Task<IActionResult> PostManagement()
		{
            var currentUser = await _userManager.GetUserAsync(User);

            var comments = await _contributionRepository.GetContributionsInprogess(currentUser.FacultyId);
			return View(comments);
		}

		//[HttpGet]
		//public async Task<IActionResult> VerifyComment()
		//{
		//    var comments = await _contributionRepository.GetContributionsInprogess();

		//    return View("VerifyComment", comments);
		//}

		[HttpGet]
		public async Task<IActionResult> Accept(string id)
		{
			var Comment = await _contributionRepository.GetContribution(id);

			if (Comment == null)
			{
				return BadRequest();
			}

			Comment.Status = Enum.BrowserComment.Accepted;
			await _contributionRepository.UpdateContribution(id, Comment);
			_emailService.SendEmail(Comment.User.Email, "Accept Contribution", "Contribution duoc chap nhan");
			return RedirectToAction("PostManagement");
		}

		[HttpGet]
		public async Task<IActionResult> Reject(string id)
		{
			var Comment = await _contributionRepository.GetContribution(id);

			if (Comment == null)
			{
				return BadRequest();
			}

			Comment.Status = Enum.BrowserComment.Rejected;
			await _contributionRepository.UpdateContribution(id, Comment);
			_emailService.SendEmail(Comment.User.Email, "Reject Contribution", "Contribution Khong duoc chap nhan");

			return RedirectToAction("PostManagement");
		}


	}
}