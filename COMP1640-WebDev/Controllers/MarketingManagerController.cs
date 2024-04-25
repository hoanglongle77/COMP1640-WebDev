using COMP1640_WebDev.Models;
using COMP1640_WebDev.Repositories;
using COMP1640_WebDev.Repositories.Interfaces;
using COMP1640_WebDev.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.IO.Compression;

namespace COMP1640_WebDev.Controllers
{

	[Authorize(Roles = "Marketing Manager")]
	public class MarketingManagerController(IWebHostEnvironment hostEnvironment, IMagazineRepository magazineRepository, IAcademicYearRepository academicYearRepository, IFacultyRepository facultyRepository) : Controller
	{
		private readonly IMagazineRepository _magazineRepository = magazineRepository;
		private readonly IAcademicYearRepository _academicYearRepository = academicYearRepository;
		private readonly IFacultyRepository _facultyRepository = facultyRepository;
		private readonly IWebHostEnvironment _hostEnvironment = hostEnvironment;

		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> DetailsMagazine(string id)
		{
			var magazineInDb = await _magazineRepository.GetMagazineByID(id);

			string imageBase64Data = Convert.ToBase64String(magazineInDb.CoverImage!);
			string image = string.Format("data:image/jpg;base64, {0}", imageBase64Data);
			ViewBag.Image = image;

			return View(magazineInDb);
		}

		public IActionResult MagazinesManagementAsync(string? attribute = null, string? value = null)
		{
			IEnumerable<MagazineTableView> magazines;
			if (!string.IsNullOrEmpty(attribute) && !string.IsNullOrEmpty(value))
			{
				magazines = _magazineRepository.SearchMagazines(attribute, value);
			}
			else
			{
				magazines = _magazineRepository.GetAllMagazines();
			}

			return View("MagazinesManagement", magazines);
		}

		[HttpGet]
		public IActionResult CreateMagazine()
		{
			var magazineViewModel = _magazineRepository.GetMagazineViewModel();
			return View(magazineViewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateMagazine(MagazineViewModel mViewModel)
		{
			if (ModelState.IsValid)
			{
				var newMagazine = new Magazine
				{
					Title = mViewModel.Title,
					Description = mViewModel.Description,
					FacultyId = mViewModel.FacultyId,
					AcademicYearId = mViewModel.AcademicYearId

				};
				await _magazineRepository.CreateMagazine(newMagazine, mViewModel.FormFile);
				TempData["AlertMessage"] = "Magazine created successfully!!!";
				return RedirectToAction("MagazinesManagement");
			}

			var magazineViewModel = _magazineRepository.GetMagazineViewModel();
			return View(magazineViewModel);
		}

		[HttpGet]
		public async Task<IActionResult> EditMagazine(string id)
		{
			var result = await _magazineRepository.GetMagazineByID(id);
			var magazineViewModel = _magazineRepository.GetMagazineViewModel();
			magazineViewModel.Id = result.Id;
			magazineViewModel.Title = result.Title!;
			magazineViewModel.Description = result.Description!;
			magazineViewModel.AcademicYearId = result.AcademicYearId;
			magazineViewModel.FacultyId = result.FacultyId;
			string imageString = Convert.ToBase64String(result.CoverImage);
			@ViewBag.Image = string.Format("data:image/jpg;base64, {0}", imageString);
			return View(magazineViewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditMagazine(MagazineViewModel mViewModel)
		{
			if (ModelState.IsValid)
			{
				var newMagazine = new Magazine
				{
					Id = mViewModel.Id!,
					Title = mViewModel.Title,
					Description = mViewModel.Description,
					FacultyId = mViewModel.FacultyId,
					AcademicYearId = mViewModel.AcademicYearId

				};
				await _magazineRepository.UpdateMagazine(newMagazine, mViewModel.FormFile);
				TempData["AlertMessage"] = "Magazine updated successfully!!!";
				return RedirectToAction("MagazinesManagement");
			}

			var magazineViewModel = _magazineRepository.GetMagazineViewModel();
			return View(magazineViewModel);
		}

		[HttpGet]
		public async Task<IActionResult> DeleteMagazine(string id)
		{
			var removedMagazine = await _magazineRepository.RemoveMagazine(id);

			if (removedMagazine == null)
			{
				TempData["AlertMessage"] = "Error: Unable to delete Magazine. Magazine not found or some other error occurred.";
			}
			else
			{
				TempData["AlertMessage"] = "Success: Magazine deleted successfully!";
			}

			return RedirectToAction("MagazinesManagement");
		}

		public IActionResult DataManagement()
		{
			var uploadsPath = Path.Combine(_hostEnvironment.WebRootPath, "images");
			var fileModels = Directory.GetFiles(uploadsPath)
									  .Select(file => Path.GetFileName(file))
									  .ToList();
			return View(fileModels);
		}


		public IActionResult DownloadZip()
		{
			var uploadsPath = Path.Combine(_hostEnvironment.WebRootPath, "images");

			var tempZipFileName = "MarketingFiles.zip";
			var tempZipPath = Path.Combine(Path.GetTempPath(), tempZipFileName);

			if (System.IO.File.Exists(tempZipPath))
			{
				System.IO.File.Delete(tempZipPath);
			}

			using (var zipStream = new FileStream(tempZipPath, FileMode.CreateNew))
			using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
			{
				var files = Directory.GetFiles(uploadsPath);
				foreach (var filePath in files)
				{
					var fileInfo = new FileInfo(filePath);
					var entry = archive.CreateEntry(fileInfo.Name);
					using (var entryStream = entry.Open())
					using (var fileStream = System.IO.File.OpenRead(filePath))
					{
						fileStream.CopyTo(entryStream);
					}
				}
			}
			return PhysicalFile(tempZipPath, "application/zip", tempZipFileName);
		}

		public IActionResult DownloadSingleFile(string file)
		{
			if (string.IsNullOrEmpty(file))
			{
				return BadRequest("Invalid file name.");
			}

			var uploadsPath = Path.Combine(_hostEnvironment.WebRootPath, "images");

			var filePath = Path.Combine(uploadsPath, file);
			if (!System.IO.File.Exists(filePath))
			{
				return NotFound();
			}

			return PhysicalFile(filePath, "application/octet-stream", file);
		}
	}
}